using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// Per-substat breakdown for an artifact: its value, how many high-roll-equivalents it represents,
/// and how useful it is for the chosen weight profile.
/// </summary>
/// <param name="Type">The substat type.</param>
/// <param name="Value">The substat value (fraction for percentage stats, absolute for flat stats).</param>
/// <param name="RollValue">Value expressed in high-roll equivalents (<c>value / max 5★ roll</c>).</param>
/// <param name="Weight">Usefulness weight for this stat, 0-1, from the active profile.</param>
/// <param name="Usefulness">Weighted contribution (<c>RollValue * Weight</c>).</param>
/// <param name="IsDead">Whether this substat is considered useless for the profile.</param>
public readonly record struct SubstatAnalysis(
    StatType Type,
    double Value,
    double RollValue,
    double Weight,
    double Usefulness,
    bool IsDead);
