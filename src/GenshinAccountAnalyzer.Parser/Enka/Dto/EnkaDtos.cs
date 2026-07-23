using System.Text.Json.Serialization;

namespace GenshinAccountAnalyzer.Parser.Enka.Dto;

// These DTOs mirror the Enka.Network showcase JSON schema 1:1. They are deliberately internal:
// nothing outside the Parser project may depend on the source's shape (see Clean Architecture rules).

/// <summary>Root of an Enka.Network response.</summary>
internal sealed class EnkaResponseDto
{
    [JsonPropertyName("playerInfo")]
    public EnkaPlayerInfoDto? PlayerInfo { get; set; }

    [JsonPropertyName("avatarInfoList")]
    public List<EnkaAvatarInfoDto>? AvatarInfoList { get; set; }

    [JsonPropertyName("uid")]
    public string? Uid { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }
}

/// <summary>Player-level info block.</summary>
internal sealed class EnkaPlayerInfoDto
{
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("worldLevel")]
    public int WorldLevel { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("finishAchievementNum")]
    public int FinishAchievementNum { get; set; }

    [JsonPropertyName("towerFloorIndex")]
    public int? TowerFloorIndex { get; set; }

    [JsonPropertyName("towerLevelIndex")]
    public int? TowerLevelIndex { get; set; }

    [JsonPropertyName("profilePicture")]
    public EnkaProfilePictureDto? ProfilePicture { get; set; }

    [JsonPropertyName("showAvatarInfoList")]
    public List<EnkaShowAvatarDto>? ShowAvatarInfoList { get; set; }
}

/// <summary>Profile picture reference.</summary>
internal sealed class EnkaProfilePictureDto
{
    [JsonPropertyName("avatarId")]
    public int? AvatarId { get; set; }
}

/// <summary>Lightweight showcase entry carrying costume/energy info not present in the detailed avatar block.</summary>
internal sealed class EnkaShowAvatarDto
{
    [JsonPropertyName("avatarId")]
    public int AvatarId { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("costumeId")]
    public int? CostumeId { get; set; }

    [JsonPropertyName("energyType")]
    public int? EnergyType { get; set; }
}

/// <summary>Detailed per-character block.</summary>
internal sealed class EnkaAvatarInfoDto
{
    [JsonPropertyName("avatarId")]
    public int AvatarId { get; set; }

    [JsonPropertyName("propMap")]
    public Dictionary<string, EnkaPropValueDto>? PropMap { get; set; }

    [JsonPropertyName("talentIdList")]
    public List<int>? TalentIdList { get; set; }

    [JsonPropertyName("fightPropMap")]
    public Dictionary<string, double>? FightPropMap { get; set; }

    [JsonPropertyName("skillLevelMap")]
    public Dictionary<string, int>? SkillLevelMap { get; set; }

    [JsonPropertyName("equipList")]
    public List<EnkaEquipDto>? EquipList { get; set; }
}

/// <summary>A value inside <c>propMap</c> (level, ascension, ...).</summary>
internal sealed class EnkaPropValueDto
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("ival")]
    public string? Ival { get; set; }

    [JsonPropertyName("val")]
    public string? Val { get; set; }
}

/// <summary>An equipped item: either a weapon or an artifact, always with a <c>flat</c> block.</summary>
internal sealed class EnkaEquipDto
{
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    [JsonPropertyName("reliquary")]
    public EnkaReliquaryDto? Reliquary { get; set; }

    [JsonPropertyName("weapon")]
    public EnkaWeaponDto? Weapon { get; set; }

    [JsonPropertyName("flat")]
    public EnkaFlatDto? Flat { get; set; }
}

/// <summary>Dynamic reliquary data (level, main/append prop ids).</summary>
internal sealed class EnkaReliquaryDto
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
}

/// <summary>Dynamic weapon data (level, ascension, refinement affixes).</summary>
internal sealed class EnkaWeaponDto
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("promoteLevel")]
    public int PromoteLevel { get; set; }

    [JsonPropertyName("affixMap")]
    public Dictionary<string, int>? AffixMap { get; set; }
}

/// <summary>Resolved (flat) item data shared by weapons and artifacts.</summary>
internal sealed class EnkaFlatDto
{
    [JsonPropertyName("nameTextMapHash")]
    public string? NameTextMapHash { get; set; }

    [JsonPropertyName("rankLevel")]
    public int RankLevel { get; set; }

    [JsonPropertyName("itemType")]
    public string? ItemType { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("equipType")]
    public string? EquipType { get; set; }

    [JsonPropertyName("setId")]
    public int? SetId { get; set; }

    [JsonPropertyName("reliquaryMainstat")]
    public EnkaReliquaryMainstatDto? ReliquaryMainstat { get; set; }

    [JsonPropertyName("reliquarySubstats")]
    public List<EnkaReliquarySubstatDto>? ReliquarySubstats { get; set; }

    [JsonPropertyName("weaponStats")]
    public List<EnkaWeaponStatDto>? WeaponStats { get; set; }
}

/// <summary>Artifact main stat.</summary>
internal sealed class EnkaReliquaryMainstatDto
{
    [JsonPropertyName("mainPropId")]
    public string? MainPropId { get; set; }

    [JsonPropertyName("statValue")]
    public double StatValue { get; set; }
}

/// <summary>Artifact sub stat.</summary>
internal sealed class EnkaReliquarySubstatDto
{
    [JsonPropertyName("appendPropId")]
    public string? AppendPropId { get; set; }

    [JsonPropertyName("statValue")]
    public double StatValue { get; set; }
}

/// <summary>Weapon stat entry (base ATK plus optional secondary).</summary>
internal sealed class EnkaWeaponStatDto
{
    [JsonPropertyName("appendPropId")]
    public string? AppendPropId { get; set; }

    [JsonPropertyName("statValue")]
    public double StatValue { get; set; }
}
