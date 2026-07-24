using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>Per-slot optimisation verdict: the equipped main stat versus the recommended one.</summary>
/// <param name="Slot">The artifact slot.</param>
/// <param name="CurrentMain">The equipped main stat (<see cref="StatType.None"/> when the slot is empty).</param>
/// <param name="RecommendedMain">The recommended main stat.</param>
/// <param name="IsOptimal">Whether the equipped main stat is an acceptable choice for the slot.</param>
public readonly record struct SlotOptimization(
    ArtifactSlot Slot,
    StatType CurrentMain,
    StatType RecommendedMain,
    bool IsOptimal);

/// <summary>
/// The optimal build target for a character: recommended artifact main stats per slot (sands, goblet,
/// circlet, ...), best-in-slot weapon, and how close the current gear is to that target.
/// </summary>
public sealed record BuildOptimization
{
    /// <summary>The in-game character (avatar) identifier.</summary>
    public required int CharacterId { get; init; }

    /// <summary>Per-slot current-versus-recommended main stat verdicts.</summary>
    public required IReadOnlyList<SlotOptimization> Slots { get; init; }

    /// <summary>Best-in-slot weapon suggestion, when available.</summary>
    public WeaponOption? BestWeapon { get; init; }

    /// <summary>Share of slots whose main stat is already optimal, 0-100.</summary>
    public required double OptimizationScore { get; init; }

    /// <summary>Bilingual notes describing the changes that would optimise the build.</summary>
    public required IReadOnlyList<LocalizedText> Notes { get; init; }
}
