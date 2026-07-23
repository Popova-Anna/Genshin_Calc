using FluentAssertions;
using GenshinAccountAnalyzer.Domain.Damage;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Calculator.Tests;

public sealed class DamageCalculatorTests
{
    private static readonly DamageCalculator Calculator = new();

    // Level-90 target, 0% resistance -> defense multiplier 0.5, resistance multiplier 1.0.
    private static EnemyProfile CleanEnemy => new() { Level = 90, Resistance = 0 };

    private static DamageInput BaseInput(double talent = 1.0, double atk = 1000) => new()
    {
        CharacterLevel = 90,
        TalentMultiplier = talent,
        ScalingStatValue = atk,
        Enemy = CleanEnemy,
    };

    [Fact]
    public void CalculateHit_BaseCase_AppliesDefenseAndResistance()
    {
        // 1.0 * 1000 * 0.5 (def) * 1.0 (res) = 500.
        DamageResult result = Calculator.CalculateHit(BaseInput());

        result.NonCritical.Should().BeApproximately(500d, 1e-6);
    }

    [Fact]
    public void CalculateHit_AppliesCritMultipliers()
    {
        DamageInput input = BaseInput() with { CritRate = 0.5, CritDamage = 1.0 };

        DamageResult result = Calculator.CalculateHit(input);

        result.NonCritical.Should().BeApproximately(500d, 1e-6);
        result.Critical.Should().BeApproximately(1000d, 1e-6);   // 500 * (1 + 1.0)
        result.Average.Should().BeApproximately(750d, 1e-6);     // 500 * (1 + 0.5 * 1.0)
    }

    [Fact]
    public void CalculateHit_AppliesDamageBonus()
    {
        DamageInput input = BaseInput() with { DamageBonus = 0.46 };

        Calculator.CalculateHit(input).NonCritical.Should().BeApproximately(500d * 1.46, 1e-6);
    }

    [Fact]
    public void CalculateHit_ForwardVaporize_AppliesOnePointFiveTimesWithEm()
    {
        // Amp = 1.5 * (1 + 2.78 * 200 / 1600) = 1.5 * 1.3475 = 2.02125.
        // base 2.0 * 2000 = 4000; * amp * 0.5 def = 4042.5.
        DamageInput input = BaseInput(talent: 2.0, atk: 2000) with
        {
            Amplifying = AmplifyingReaction.Vaporize,
            TriggerElement = ElementType.Pyro,
            ElementalMastery = 200,
        };

        Calculator.CalculateHit(input).NonCritical.Should().BeApproximately(4042.5, 1e-3);
    }

    [Fact]
    public void CalculateHit_ReverseVaporize_UsesTwoTimesBase()
    {
        DamageInput forward = BaseInput(talent: 1.0, atk: 1000) with
        {
            Amplifying = AmplifyingReaction.Vaporize,
            TriggerElement = ElementType.Pyro,
        };
        DamageInput reverse = forward with { TriggerElement = ElementType.Hydro };

        double forwardDmg = Calculator.CalculateHit(forward).NonCritical;
        double reverseDmg = Calculator.CalculateHit(reverse).NonCritical;

        // 2.0 base vs 1.5 base (EM 0) -> ratio 2/1.5.
        (reverseDmg / forwardDmg).Should().BeApproximately(2d / 1.5d, 1e-6);
    }

    [Fact]
    public void CalculateHit_Aggravate_AddsFlatTermToBaseDamage()
    {
        // Additive term = 1.15 * (1 + 5*200/1400) * 1446.8535 = 2852.40; * 0.5 def = 1426.20.
        DamageInput input = new()
        {
            CharacterLevel = 90,
            TalentMultiplier = 0,
            ScalingStatValue = 0,
            Additive = AdditiveReaction.Aggravate,
            ElementalMastery = 200,
            Enemy = CleanEnemy,
        };

        Calculator.CalculateHit(input).NonCritical.Should().BeApproximately(1426.20, 0.1);
    }

    [Fact]
    public void CalculateHit_UsesProvidedScalingStatValue_ForHpScaling()
    {
        // HP scaling: 0.05 * 30000 = 1500 base; * 0.5 def = 750.
        DamageInput input = new()
        {
            CharacterLevel = 90,
            TalentMultiplier = 0.05,
            Scaling = ScalingType.Hp,
            ScalingStatValue = 30000,
            Enemy = CleanEnemy,
        };

        Calculator.CalculateHit(input).NonCritical.Should().BeApproximately(750d, 1e-6);
    }

    [Fact]
    public void CalculateTransformative_Overload_MatchesFormula()
    {
        // 2.0 * (1 + 0) * 1446.8535 * 0.9 (res 0.1) = 2604.336.
        double damage = Calculator.CalculateTransformative(
            TransformativeReaction.Overloaded, 90, elementalMastery: 0, reactionBonus: 0, EnemyProfile.Standard90);

        damage.Should().BeApproximately(2604.336, 0.01);
    }

    [Fact]
    public void CalculateTransformative_Hyperbloom_WithEm_MatchesFormula()
    {
        // 3.0 * (1 + 16*200/2200) * 1446.8535 * 0.9 = 9588.69.
        double damage = Calculator.CalculateTransformative(
            TransformativeReaction.Hyperbloom, 90, elementalMastery: 200, reactionBonus: 0, EnemyProfile.Standard90);

        damage.Should().BeApproximately(9588.69, 0.1);
    }
}
