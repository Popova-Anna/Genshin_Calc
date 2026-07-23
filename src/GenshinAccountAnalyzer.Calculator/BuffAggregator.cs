using GenshinAccountAnalyzer.Domain.Damage;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Calculator;

/// <summary>
/// Stacks buffs additively within each stat, the way most Genshin bonuses combine, to produce the
/// effective stats a <see cref="DamageInput"/> needs. Which buffs are included models snapshot vs
/// dynamic behaviour — the caller decides which are active at calculation time.
/// </summary>
public static class BuffAggregator
{
    /// <summary>Sums all flat modifiers for a stat across the buffs.</summary>
    /// <param name="stat">The stat to total.</param>
    /// <param name="buffs">The active buffs.</param>
    public static double SumFlat(StatType stat, IEnumerable<Buff> buffs) =>
        Sum(stat, ModifierKind.Flat, buffs);

    /// <summary>Sums all percentage modifiers for a stat across the buffs (as a fraction).</summary>
    /// <param name="stat">The stat to total.</param>
    /// <param name="buffs">The active buffs.</param>
    public static double SumPercent(StatType stat, IEnumerable<Buff> buffs) =>
        Sum(stat, ModifierKind.Percent, buffs);

    /// <summary>
    /// Computes an effective scaling stat: <c>base × (1 + Σ percent) + Σ flat</c>. Percentage bonuses
    /// apply to the base value only, matching the game (e.g. ATK% scales base ATK, not flat ATK).
    /// </summary>
    /// <param name="baseValue">The character+weapon base value.</param>
    /// <param name="percentStat">The percentage stat (e.g. <see cref="StatType.AtkPercent"/>).</param>
    /// <param name="flatStat">The flat stat (e.g. <see cref="StatType.Atk"/>).</param>
    /// <param name="buffs">The active buffs.</param>
    public static double EffectiveStat(
        double baseValue,
        StatType percentStat,
        StatType flatStat,
        IEnumerable<Buff> buffs)
    {
        IReadOnlyList<Buff> materialized = buffs as IReadOnlyList<Buff> ?? buffs.ToList();
        return baseValue * (1d + SumPercent(percentStat, materialized)) + SumFlat(flatStat, materialized);
    }

    /// <summary>Computes effective total ATK from base ATK and buffs.</summary>
    /// <param name="baseAttack">Character base ATK plus weapon base ATK.</param>
    /// <param name="buffs">The active buffs.</param>
    public static double EffectiveAttack(double baseAttack, IEnumerable<Buff> buffs) =>
        EffectiveStat(baseAttack, StatType.AtkPercent, StatType.Atk, buffs);

    private static double Sum(StatType stat, ModifierKind kind, IEnumerable<Buff> buffs) =>
        buffs
            .SelectMany(buff => buff.Modifiers)
            .Where(modifier => modifier.Stat == stat && modifier.Kind == kind)
            .Sum(modifier => modifier.Value);
}
