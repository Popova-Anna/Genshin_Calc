using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// The default, role-agnostic substat usefulness profile and the dead-roll threshold. Role-specific
/// profiles (Stage 7) will replace the default per character; the values here are a sensible baseline
/// for an offensive crit build.
/// </summary>
public static class SubstatWeights
{
    /// <summary>Weight at or below which a substat counts as a "dead" roll.</summary>
    public const double DeadWeightThreshold = 0d;

    /// <summary>
    /// A generic offensive profile: crit stats are prime; ATK%/EM/ER are useful; flat and defensive
    /// stats are largely wasted. Flat DEF is treated as fully dead.
    /// </summary>
    public static SubstatWeightProfile DefaultProfile { get; } = new(
        "Generic Crit DPS",
        new Dictionary<StatType, double>
        {
            [StatType.CritRate] = 1.0d,
            [StatType.CritDamage] = 1.0d,
            [StatType.AtkPercent] = 0.8d,
            [StatType.ElementalMastery] = 0.8d,
            [StatType.EnergyRecharge] = 0.5d,
            [StatType.Atk] = 0.3d,
            [StatType.HpPercent] = 0.3d,
            [StatType.DefPercent] = 0.1d,
            [StatType.Hp] = 0.1d,
            [StatType.Def] = 0.0d,
        });
}
