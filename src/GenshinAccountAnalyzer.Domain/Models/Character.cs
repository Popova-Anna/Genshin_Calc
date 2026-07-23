using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Models;

/// <summary>
/// A character on the account, with equipped gear and final computed statistics,
/// in the analyzer's own source-independent representation.
/// </summary>
public sealed record Character
{
    /// <summary>The in-game character (avatar) identifier.</summary>
    public required int Id { get; init; }

    /// <summary>Display name, or a generated placeholder when metadata is unavailable.</summary>
    public required string Name { get; init; }

    /// <summary>The character's element (vision).</summary>
    public ElementType Element { get; init; } = ElementType.Unknown;

    /// <summary>The weapon type the character wields.</summary>
    public WeaponType WeaponType { get; init; } = WeaponType.Unknown;

    /// <summary>Rarity (star rating), 4 or 5.</summary>
    public int Rarity { get; init; }

    /// <summary>Current level, 1-90.</summary>
    public required int Level { get; init; }

    /// <summary>Ascension phase, 0-6.</summary>
    public int Ascension { get; init; }

    /// <summary>Number of unlocked constellations, 0-6.</summary>
    public int ConstellationLevel { get; init; }

    /// <summary>Equipped costume identifier, when one is applied.</summary>
    public int? CostumeId { get; init; }

    /// <summary>The active talents (raw skill levels).</summary>
    public required IReadOnlyList<Talent> Talents { get; init; }

    /// <summary>The equipped weapon, when present.</summary>
    public Weapon? Weapon { get; init; }

    /// <summary>The equipped artifacts (0-5).</summary>
    public required IReadOnlyList<Artifact> Artifacts { get; init; }

    /// <summary>Final, computed statistics as reported by the data source.</summary>
    public required StatSheet Stats { get; init; }
}
