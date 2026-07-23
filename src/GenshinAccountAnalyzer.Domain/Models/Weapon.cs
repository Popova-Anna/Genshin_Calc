using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Models;

/// <summary>
/// A weapon equipped by a character, in the analyzer's own source-independent representation.
/// </summary>
public sealed record Weapon
{
    /// <summary>The in-game weapon identifier.</summary>
    public required int Id { get; init; }

    /// <summary>Display name, or a generated placeholder when metadata is unavailable.</summary>
    public required string Name { get; init; }

    /// <summary>The weapon category.</summary>
    public required WeaponType Type { get; init; }

    /// <summary>Rarity (star rating), 1-5.</summary>
    public required int Rarity { get; init; }

    /// <summary>Current level, 1-90.</summary>
    public required int Level { get; init; }

    /// <summary>Ascension phase, 0-6.</summary>
    public required int Ascension { get; init; }

    /// <summary>Refinement rank, 1-5.</summary>
    public required int Refinement { get; init; }

    /// <summary>Base ATK provided by the weapon at its current level.</summary>
    public required double BaseAttack { get; init; }

    /// <summary>The weapon's secondary (sub) stat, when it has one.</summary>
    public Stat? SecondaryStat { get; init; }
}
