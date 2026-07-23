using FluentAssertions;
using GenshinAccountAnalyzer.Domain.Damage;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Calculator.Tests;

public sealed class BuffAggregatorTests
{
    [Fact]
    public void EffectiveAttack_StacksPercentOnBaseAndAddsFlat()
    {
        var buffs = new List<Buff>
        {
            new("Weapon", [new StatModifier(StatType.AtkPercent, 0.20, ModifierKind.Percent)]),
            new("Noblesse", [new StatModifier(StatType.AtkPercent, 0.20, ModifierKind.Percent)]),
            new("Bennett", [new StatModifier(StatType.Atk, 500, ModifierKind.Flat)]),
        };

        // 800 * (1 + 0.4) + 500 = 1620.
        BuffAggregator.EffectiveAttack(800, buffs).Should().BeApproximately(1620d, 1e-6);
    }

    [Fact]
    public void SumFlat_TotalsFlatModifiersForStat()
    {
        var buffs = new List<Buff>
        {
            new("A", [new StatModifier(StatType.CritDamage, 0.5, ModifierKind.Flat)]),
            new("B", [new StatModifier(StatType.CritDamage, 0.2, ModifierKind.Flat)]),
            new("C", [new StatModifier(StatType.ElementalMastery, 200, ModifierKind.Flat)]),
        };

        BuffAggregator.SumFlat(StatType.CritDamage, buffs).Should().BeApproximately(0.7, 1e-9);
        BuffAggregator.SumFlat(StatType.ElementalMastery, buffs).Should().BeApproximately(200, 1e-9);
    }

    [Fact]
    public void SumPercent_IgnoresFlatModifiers()
    {
        var buffs = new List<Buff>
        {
            new("A", [new StatModifier(StatType.PyroDamageBonus, 0.15, ModifierKind.Percent)]),
            new("B", [new StatModifier(StatType.PyroDamageBonus, 0.30, ModifierKind.Flat)]),
        };

        BuffAggregator.SumPercent(StatType.PyroDamageBonus, buffs).Should().BeApproximately(0.15, 1e-9);
    }
}
