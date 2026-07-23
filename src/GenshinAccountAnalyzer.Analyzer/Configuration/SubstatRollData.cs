using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Highest 5★ substat roll values, in the same units the domain stores stats (fractions for percentage
/// stats, absolute values for flat stats). A substat's roll value is <c>value / max roll</c>. These are
/// fixed game constants.
/// </summary>
public static class SubstatRollData
{
    /// <summary>The four 5★ roll tiers are 70%, 80%, 90% and 100% of the maximum roll.</summary>
    public const double LowestRollFraction = 0.7d;

    private static readonly IReadOnlyDictionary<StatType, double> MaxRolls = new Dictionary<StatType, double>
    {
        [StatType.Hp] = 298.75d,
        [StatType.Atk] = 19.45d,
        [StatType.Def] = 23.15d,
        [StatType.HpPercent] = 0.0583d,
        [StatType.AtkPercent] = 0.0583d,
        [StatType.DefPercent] = 0.0729d,
        [StatType.ElementalMastery] = 23.31d,
        [StatType.EnergyRecharge] = 0.0648d,
        [StatType.CritRate] = 0.0389d,
        [StatType.CritDamage] = 0.0777d,
    };

    /// <summary>Gets the maximum 5★ roll value for a substat, or <c>0</c> when the stat cannot roll.</summary>
    /// <param name="type">The substat type.</param>
    /// <returns>The maximum single-roll value.</returns>
    public static double MaxRoll(StatType type) => MaxRolls.TryGetValue(type, out double value) ? value : 0d;

    /// <summary>Returns whether <paramref name="type"/> is a valid artifact substat.</summary>
    /// <param name="type">The stat type.</param>
    public static bool CanRoll(StatType type) => MaxRolls.ContainsKey(type);
}
