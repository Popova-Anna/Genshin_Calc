using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Coefficients for the stat-based weapon score used to rank weapons. The score is a transparent proxy
/// for damage — base ATK plus a usefulness-weighted secondary stat — that will be superseded by exact,
/// passive-aware DPS once the damage calculator (Stage 9) exists.
/// </summary>
public static class WeaponScoreWeights
{
    /// <summary>Weight of the base-ATK component (base-ATK and secondary weights sum to 1.0).</summary>
    public const double BaseAttackWeight = 0.6d;

    /// <summary>Weight of the secondary-stat component.</summary>
    public const double SecondaryWeight = 0.4d;

    /// <summary>Base ATK that normalises the base-ATK component to ~1.0 (a strong 5★ weapon).</summary>
    public const double ReferenceBaseAttack = 608d;

    /// <summary>Minimum rarity a weapon must have to appear as a recommendation.</summary>
    public const int MinRecommendationRarity = 4;

    /// <summary>How many ranked options to return (best-in-slot plus alternatives).</summary>
    public const int RecommendationCount = 3;

    // Representative "strong 5★" secondary values, normalising each secondary component to ~1.0 so
    // different secondary stats are comparable. Percentage stats are fractions.
    private static readonly IReadOnlyDictionary<StatType, double> SecondaryReferences =
        new Dictionary<StatType, double>
        {
            [StatType.CritRate] = 0.331d,
            [StatType.CritDamage] = 0.662d,
            [StatType.AtkPercent] = 0.496d,
            [StatType.HpPercent] = 0.496d,
            [StatType.DefPercent] = 0.692d,
            [StatType.ElementalMastery] = 198d,
            [StatType.EnergyRecharge] = 0.551d,
            [StatType.PhysicalDamageBonus] = 0.517d,
        };

    /// <summary>Gets the normalising reference value for a secondary stat, or <c>0</c> when it has none.</summary>
    /// <param name="type">The secondary stat type.</param>
    public static double SecondaryReference(StatType type) =>
        SecondaryReferences.TryGetValue(type, out double value) ? value : 0d;
}
