using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Exceptions;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;
using GenshinAccountAnalyzer.Parser.Enka.Dto;
using GenshinAccountAnalyzer.Parser.Enka.Mapping;

namespace GenshinAccountAnalyzer.Parser.Enka;

/// <summary>
/// Imports Enka.Network showcase exports into the internal <see cref="Account"/> model.
/// The raw Enka DTOs never leave this project — everything returned is the analyzer's own model.
/// </summary>
public sealed class EnkaImporter : IAccountImporter
{
    /// <summary>Enka stores artifact levels one-based (a +20 artifact reports level 21).</summary>
    private const int ArtifactLevelOffset = 1;

    /// <summary>Refinement rank is one-based; Enka stores the affix level zero-based.</summary>
    private const int RefinementOffset = 1;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private readonly IGameMetadataProvider _metadata;

    /// <summary>Initializes the importer with a metadata provider used to enrich names/elements.</summary>
    /// <param name="metadata">The game metadata provider.</param>
    public EnkaImporter(IGameMetadataProvider metadata)
    {
        _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <inheritdoc />
    public ImportSource Source => ImportSource.Enka;

    /// <inheritdoc />
    public async Task<Account> ImportAsync(Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        EnkaResponseDto? response;
        try
        {
            response = await JsonSerializer
                .DeserializeAsync<EnkaResponseDto>(stream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException exception)
        {
            throw new AccountImportException("The provided content is not valid Enka.Network JSON.", exception);
        }

        if (response?.PlayerInfo is null)
        {
            throw new AccountImportException("Enka content is missing the required 'playerInfo' block.");
        }

        // showAvatarInfoList carries costume/energy details absent from the detailed avatar blocks.
        Dictionary<int, EnkaShowAvatarDto> showcase = (response.PlayerInfo.ShowAvatarInfoList ?? [])
            .GroupBy(entry => entry.AvatarId)
            .ToDictionary(group => group.Key, group => group.First());

        List<Character> characters = (response.AvatarInfoList ?? [])
            .Select(avatar => MapCharacter(avatar, showcase))
            .ToList();

        return new Account
        {
            Uid = response.Uid ?? string.Empty,
            Profile = MapProfile(response.PlayerInfo),
            Characters = characters,
            Source = ImportSource.Enka,
            ImportedAt = DateTimeOffset.UtcNow,
        };
    }

    private static Profile MapProfile(EnkaPlayerInfoDto player) => new()
    {
        Nickname = player.Nickname ?? string.Empty,
        AdventureRank = player.Level,
        WorldLevel = player.WorldLevel,
        Signature = player.Signature,
        Achievements = player.FinishAchievementNum,
        SpiralAbyssFloor = player.TowerFloorIndex,
        SpiralAbyssChamber = player.TowerLevelIndex,
        ProfilePictureCharacterId = player.ProfilePicture?.AvatarId,
    };

    private Character MapCharacter(EnkaAvatarInfoDto avatar, IReadOnlyDictionary<int, EnkaShowAvatarDto> showcase)
    {
        CharacterMetadata? metadata = _metadata.GetCharacter(avatar.AvatarId);

        List<EnkaEquipDto> equipment = avatar.EquipList ?? [];
        Weapon? weapon = equipment
            .Where(equip => equip.Weapon is not null && equip.Flat is not null)
            .Select(MapWeapon)
            .FirstOrDefault();

        List<Artifact> artifacts = equipment
            .Where(equip => equip.Reliquary is not null && equip.Flat is not null)
            .Select(MapArtifact)
            .ToList();

        List<Talent> talents = (avatar.SkillLevelMap ?? [])
            .Select(kv => new Talent(ParseInt(kv.Key), kv.Value))
            .ToList();

        showcase.TryGetValue(avatar.AvatarId, out EnkaShowAvatarDto? show);

        // The weapon type derived from the equipped weapon's icon is authoritative; metadata is a fallback.
        WeaponType weaponType = weapon?.Type is { } type and not WeaponType.Unknown
            ? type
            : metadata?.Weapon ?? WeaponType.Unknown;

        return new Character
        {
            Id = avatar.AvatarId,
            Name = metadata?.Name ?? $"Character {avatar.AvatarId}",
            Element = metadata?.Element ?? ElementType.Unknown,
            WeaponType = weaponType,
            Rarity = metadata?.Rarity ?? 0,
            Level = ReadPropMap(avatar.PropMap, FightPropId.PropTypeLevel),
            Ascension = ReadPropMap(avatar.PropMap, FightPropId.PropTypeAscension),
            ConstellationLevel = avatar.TalentIdList?.Count ?? 0,
            CostumeId = show?.CostumeId,
            Talents = talents,
            Weapon = weapon,
            Artifacts = artifacts,
            Stats = BuildStatSheet(avatar.FightPropMap),
        };
    }

    private Weapon MapWeapon(EnkaEquipDto equip)
    {
        EnkaWeaponDto data = equip.Weapon!;
        EnkaFlatDto flat = equip.Flat!;
        WeaponMetadata? metadata = _metadata.GetWeapon(equip.ItemId);

        List<EnkaWeaponStatDto> stats = flat.WeaponStats ?? [];
        double baseAttack = stats
            .Where(stat => stat.AppendPropId == EnkaStatMaps.WeaponBaseAttackProp)
            .Select(stat => stat.StatValue)
            .FirstOrDefault();

        Stat? secondary = stats
            .Where(stat => stat.AppendPropId != EnkaStatMaps.WeaponBaseAttackProp)
            .Select(stat => EnkaStatMaps.ResolveFlatStat(stat.AppendPropId, stat.StatValue))
            .Where(resolved => resolved.Type != StatType.None)
            .Select(resolved => (Stat?)new Stat(resolved.Type, resolved.Value))
            .FirstOrDefault();

        int refinement = (data.AffixMap?.Values.FirstOrDefault() ?? 0) + RefinementOffset;

        return new Weapon
        {
            Id = equip.ItemId,
            Name = metadata?.Name ?? $"Weapon {equip.ItemId}",
            Type = EnkaStatMaps.InferWeaponType(flat.Icon) is var inferred and not WeaponType.Unknown
                ? inferred
                : metadata?.Type ?? WeaponType.Unknown,
            Rarity = flat.RankLevel,
            Level = data.Level,
            Ascension = data.PromoteLevel,
            Refinement = refinement,
            BaseAttack = baseAttack,
            SecondaryStat = secondary,
        };
    }

    private Artifact MapArtifact(EnkaEquipDto equip)
    {
        EnkaReliquaryDto reliquary = equip.Reliquary!;
        EnkaFlatDto flat = equip.Flat!;
        int setId = flat.SetId ?? 0;

        (StatType mainType, double mainValue) = EnkaStatMaps.ResolveFlatStat(
            flat.ReliquaryMainstat?.MainPropId,
            flat.ReliquaryMainstat?.StatValue ?? 0d);

        List<Stat> subStats = (flat.ReliquarySubstats ?? [])
            .Select(sub => EnkaStatMaps.ResolveFlatStat(sub.AppendPropId, sub.StatValue))
            .Where(resolved => resolved.Type != StatType.None)
            .Select(resolved => new Stat(resolved.Type, resolved.Value))
            .ToList();

        ArtifactSlot slot = flat.EquipType is not null
            && EnkaStatMaps.EquipTypeToSlot.TryGetValue(flat.EquipType, out ArtifactSlot mapped)
                ? mapped
                : ArtifactSlot.Unknown;

        return new Artifact
        {
            Id = equip.ItemId,
            SetId = setId,
            SetName = _metadata.GetArtifactSetName(setId) ?? $"Set {setId}",
            Slot = slot,
            Rarity = flat.RankLevel,
            Level = Math.Max(0, reliquary.Level - ArtifactLevelOffset),
            MainStat = new Stat(mainType, mainValue),
            SubStats = subStats,
            RollCount = reliquary.AppendPropIdList?.Count ?? 0,
        };
    }

    private static StatSheet BuildStatSheet(IReadOnlyDictionary<string, double>? fightPropMap)
    {
        if (fightPropMap is null)
        {
            return StatSheet.Empty;
        }

        var values = new Dictionary<StatType, double>();
        foreach ((string key, double value) in fightPropMap)
        {
            if (int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out int propId)
                && FightPropId.ToStatType.TryGetValue(propId, out StatType statType))
            {
                values[statType] = value;
            }
        }

        return new StatSheet(values);
    }

    private static int ReadPropMap(IReadOnlyDictionary<string, EnkaPropValueDto>? propMap, int type)
    {
        if (propMap is null
            || !propMap.TryGetValue(type.ToString(CultureInfo.InvariantCulture), out EnkaPropValueDto? entry))
        {
            return 0;
        }

        return ParseInt(entry.Val ?? entry.Ival);
    }

    private static int ParseInt(string? value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
}
