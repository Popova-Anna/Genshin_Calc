using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// The result of analyzing a single artifact: crit value, roll value, roll efficiency, dead rolls and
/// a per-substat usefulness breakdown, all evaluated against a <see cref="SubstatWeightProfile"/>.
/// </summary>
public sealed record ArtifactAnalysis
{
    /// <summary>The in-game artifact identifier.</summary>
    public required int ArtifactId { get; init; }

    /// <summary>The slot the artifact occupies.</summary>
    public required ArtifactSlot Slot { get; init; }

    /// <summary>Set display name.</summary>
    public required string SetName { get; init; }

    /// <summary>Enhancement level.</summary>
    public required int Level { get; init; }

    /// <summary>Rarity (star rating).</summary>
    public required int Rarity { get; init; }

    /// <summary>Crit Value from substats (<c>2 * CritRate% + CritDamage%</c>).</summary>
    public required double CritValue { get; init; }

    /// <summary>Total roll value in high-roll equivalents, as a percentage (e.g. <c>700</c> = 7 high rolls).</summary>
    public required double RollValue { get; init; }

    /// <summary>Number of substat rolls the artifact received (0 when unknown).</summary>
    public required int RollCount { get; init; }

    /// <summary>Average roll quality: total roll value divided by roll count, as a percentage (70-100).</summary>
    public required double Efficiency { get; init; }

    /// <summary>Estimated number of rolls spent on useless substats, in high-roll equivalents.</summary>
    public required double DeadRolls { get; init; }

    /// <summary>Per-substat usefulness breakdown.</summary>
    public required IReadOnlyList<SubstatAnalysis> Substats { get; init; }

    /// <summary>The name of the weight profile used to evaluate usefulness.</summary>
    public required string ProfileName { get; init; }
}
