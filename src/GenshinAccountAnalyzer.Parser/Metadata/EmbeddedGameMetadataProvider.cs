using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Parser.Metadata;

/// <summary>
/// A <see cref="IGameMetadataProvider"/> and <see cref="IWeaponCatalog"/> backed by embedded JSON data
/// files. Because it is data-driven, supporting new characters/weapons/sets is a data change (edit the
/// resource) — never a code change, satisfying the "add new content without changing architecture"
/// requirement. Unknown ids resolve to <see langword="null"/> so importers fall back to safe placeholders.
/// </summary>
public sealed class EmbeddedGameMetadataProvider : IGameMetadataProvider, IWeaponCatalog
{
    private readonly IReadOnlyDictionary<int, CharacterMetadata> _characters;
    private readonly IReadOnlyDictionary<int, WeaponInfo> _weapons;
    private readonly IReadOnlyDictionary<int, string> _artifactSets;
    private readonly IReadOnlyDictionary<WeaponType, IReadOnlyList<WeaponInfo>> _weaponsByType;

    /// <summary>Initializes a provider from explicit metadata maps (used by tests and custom hosts).</summary>
    /// <param name="characters">Character metadata keyed by avatar id.</param>
    /// <param name="weapons">Weapon catalog info keyed by weapon id.</param>
    /// <param name="artifactSets">Artifact set names keyed by set id.</param>
    public EmbeddedGameMetadataProvider(
        IReadOnlyDictionary<int, CharacterMetadata> characters,
        IReadOnlyDictionary<int, WeaponInfo> weapons,
        IReadOnlyDictionary<int, string> artifactSets)
    {
        _characters = characters ?? throw new ArgumentNullException(nameof(characters));
        _weapons = weapons ?? throw new ArgumentNullException(nameof(weapons));
        _artifactSets = artifactSets ?? throw new ArgumentNullException(nameof(artifactSets));

        _weaponsByType = _weapons.Values
            .GroupBy(weapon => weapon.Type)
            .ToDictionary(group => group.Key, IReadOnlyList<WeaponInfo> (group) => group.ToList());
    }

    /// <inheritdoc />
    public CharacterMetadata? GetCharacter(int avatarId) =>
        _characters.TryGetValue(avatarId, out CharacterMetadata? value) ? value : null;

    /// <inheritdoc />
    public WeaponMetadata? GetWeapon(int weaponId) =>
        _weapons.TryGetValue(weaponId, out WeaponInfo? value)
            ? new WeaponMetadata(value.Name, value.Type, value.Rarity)
            : null;

    /// <inheritdoc />
    public string? GetArtifactSetName(int setId) =>
        _artifactSets.TryGetValue(setId, out string? value) ? value : null;

    /// <inheritdoc />
    WeaponInfo? IWeaponCatalog.Get(int weaponId) =>
        _weapons.TryGetValue(weaponId, out WeaponInfo? value) ? value : null;

    /// <inheritdoc />
    IReadOnlyList<WeaponInfo> IWeaponCatalog.GetByType(WeaponType type) =>
        _weaponsByType.TryGetValue(type, out IReadOnlyList<WeaponInfo>? value) ? value : [];

    private const string CharactersResource = "GenshinAccountAnalyzer.Parser.Resources.characters.json";
    private const string WeaponsResource = "GenshinAccountAnalyzer.Parser.Resources.weapons.json";
    private const string ArtifactSetsResource = "GenshinAccountAnalyzer.Parser.Resources.artifactSets.json";

    private static readonly JsonSerializerOptions ResourceOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Creates a provider populated from the embedded metadata resources shipped with the parser.
    /// Missing or empty resources yield an empty provider that always falls back to placeholders.
    /// </summary>
    /// <returns>A ready-to-use provider.</returns>
    public static EmbeddedGameMetadataProvider CreateDefault()
    {
        Assembly assembly = typeof(EmbeddedGameMetadataProvider).Assembly;

        Dictionary<int, CharacterMetadataDto> characters =
            LoadResource<Dictionary<int, CharacterMetadataDto>>(assembly, CharactersResource) ?? [];
        Dictionary<int, WeaponInfoDto> weapons =
            LoadResource<Dictionary<int, WeaponInfoDto>>(assembly, WeaponsResource) ?? [];
        Dictionary<int, string> sets =
            LoadResource<Dictionary<int, string>>(assembly, ArtifactSetsResource) ?? [];

        return new EmbeddedGameMetadataProvider(
            characters.ToDictionary(kv => kv.Key, kv => kv.Value.ToMetadata()),
            weapons.ToDictionary(kv => kv.Key, kv => kv.Value.ToInfo(kv.Key)),
            sets);
    }

    private static T? LoadResource<T>(Assembly assembly, string resourceName)
        where T : class
    {
        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(stream, ResourceOptions);
    }

    private sealed record CharacterMetadataDto(
        string Name,
        ElementType Element,
        WeaponType Weapon,
        int Rarity,
        IReadOnlyList<int>? Talents)
    {
        public CharacterMetadata ToMetadata() =>
            new(Name, Element, Weapon, Rarity) { TalentSkillIds = Talents ?? [] };
    }

    private sealed record WeaponInfoDto(
        string Name,
        WeaponType Type,
        int Rarity,
        double BaseAttack,
        StatType SecondaryStat,
        double SecondaryValue)
    {
        public WeaponInfo ToInfo(int id) =>
            new(id, Name, Type, Rarity, BaseAttack, SecondaryStat, SecondaryValue);
    }
}
