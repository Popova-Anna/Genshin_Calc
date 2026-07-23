using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Resolves static, patch-versioned game metadata (names, elements, weapon types, rarities) that is not
/// present in a raw export in a directly usable form. Importers use this to enrich the internal model.
/// Implementations back this with embedded data files, remote metadata, etc. — swapping the data source
/// never requires changing importers or the domain.
/// </summary>
public interface IGameMetadataProvider
{
    /// <summary>Resolves metadata for a character by its avatar id.</summary>
    /// <param name="avatarId">The in-game avatar identifier.</param>
    /// <returns>The metadata, or <see langword="null"/> when the character is unknown.</returns>
    CharacterMetadata? GetCharacter(int avatarId);

    /// <summary>Resolves metadata for a weapon by its item id.</summary>
    /// <param name="weaponId">The in-game weapon identifier.</param>
    /// <returns>The metadata, or <see langword="null"/> when the weapon is unknown.</returns>
    WeaponMetadata? GetWeapon(int weaponId);

    /// <summary>Resolves the display name of an artifact set by its set id.</summary>
    /// <param name="setId">The in-game artifact set identifier.</param>
    /// <returns>The set name, or <see langword="null"/> when the set is unknown.</returns>
    string? GetArtifactSetName(int setId);
}

/// <summary>Static metadata describing a character.</summary>
/// <param name="Name">Localized display name.</param>
/// <param name="Element">The character's element (vision).</param>
/// <param name="Weapon">The weapon type the character wields.</param>
/// <param name="Rarity">Rarity (star rating).</param>
public sealed record CharacterMetadata(string Name, ElementType Element, WeaponType Weapon, int Rarity)
{
    /// <summary>
    /// Skill ids of the three main upgradable talents (Normal Attack, Elemental Skill, Elemental Burst),
    /// matching the keys used in an export's skill-level map. Empty when unknown.
    /// </summary>
    public IReadOnlyList<int> TalentSkillIds { get; init; } = [];
}

/// <summary>Static metadata describing a weapon.</summary>
/// <param name="Name">Localized display name.</param>
/// <param name="Type">The weapon category.</param>
/// <param name="Rarity">Rarity (star rating).</param>
public sealed record WeaponMetadata(string Name, WeaponType Type, int Rarity);
