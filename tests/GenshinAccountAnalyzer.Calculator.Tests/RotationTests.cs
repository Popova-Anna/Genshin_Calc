using FluentAssertions;
using GenshinAccountAnalyzer.Domain.Damage;

namespace GenshinAccountAnalyzer.Calculator.Tests;

public sealed class RotationTests
{
    private static readonly DamageCalculator Calculator = new();
    private static EnemyProfile CleanEnemy => new() { Level = 90, Resistance = 0 };

    private static DamageInput SimpleHit(double talent, double atk, double critRate = 0, double critDamage = 0) => new()
    {
        CharacterLevel = 90,
        TalentMultiplier = talent,
        ScalingStatValue = atk,
        CritRate = critRate,
        CritDamage = critDamage,
        Enemy = CleanEnemy,
    };

    [Fact]
    public void CalculateRotation_SumsMultipleHits()
    {
        // Each hit: 1.0 * 1000 * 0.5 def = 500 non-crit.
        var rotation = new Rotation
        {
            Name = "Two normal attacks",
            Steps =
            [
                new RotationStep { Name = "N1", Hit = SimpleHit(1.0, 1000) },
                new RotationStep { Name = "N2", Hit = SimpleHit(1.0, 1000) },
            ],
        };

        RotationResult result = Calculator.CalculateRotation(rotation);

        result.Steps.Should().HaveCount(2);
        result.TotalNonCritical.Should().BeApproximately(1000d, 1e-6);
        result.TotalCritical.Should().BeApproximately(1000d, 1e-6); // no crit dmg configured -> same as non-crit
        result.TotalAverage.Should().BeApproximately(1000d, 1e-6);
    }

    [Fact]
    public void CalculateRotation_MaxEqualsAllCritSum_FloorEqualsNoCritSum()
    {
        var rotation = new Rotation
        {
            Name = "Crit rotation",
            Steps =
            [
                new RotationStep { Name = "Skill", Hit = SimpleHit(2.0, 1000, critRate: 0.5, critDamage: 1.0) },
                new RotationStep { Name = "Burst", Hit = SimpleHit(3.0, 1000, critRate: 0.5, critDamage: 1.0) },
            ],
        };

        RotationResult result = Calculator.CalculateRotation(rotation);

        // Skill: base 1000, Burst: base 1500. Non-crit sum 2500; crit sum (x2) 5000; average (x1.5) 3750.
        result.TotalNonCritical.Should().BeApproximately(2500d, 1e-6);
        result.TotalCritical.Should().BeApproximately(5000d, 1e-6);
        result.TotalAverage.Should().BeApproximately(3750d, 1e-6);
        result.TotalAverage.Should().BeGreaterThan(result.TotalNonCritical).And.BeLessThan(result.TotalCritical);
    }

    [Fact]
    public void CalculateRotation_IncludesTransformativeProcs_WithoutCritVariance()
    {
        var rotation = new Rotation
        {
            Name = "Hyperbloom rotation",
            Steps =
            [
                new RotationStep { Name = "Hit", Hit = SimpleHit(1.0, 1000, critRate: 1.0, critDamage: 1.0) },
                new RotationStep
                {
                    Name = "Hyperbloom proc",
                    Transformative = new TransformativeHit
                    {
                        Reaction = TransformativeReaction.Hyperbloom,
                        CharacterLevel = 90,
                        ElementalMastery = 0,
                        Enemy = EnemyProfile.Standard90,
                    },
                },
            ],
        };

        RotationResult result = Calculator.CalculateRotation(rotation);

        double expectedProc = Calculator.CalculateTransformative(
            TransformativeReaction.Hyperbloom, 90, 0, 0, EnemyProfile.Standard90);

        result.Steps[1].Damage.NonCritical.Should().BeApproximately(expectedProc, 1e-6);
        result.Steps[1].Damage.Critical.Should().BeApproximately(expectedProc, 1e-6);
        result.Steps[1].Damage.Average.Should().BeApproximately(expectedProc, 1e-6);

        // Hit is guaranteed crit (rate 1.0): non-crit 500, crit/average 1000. Plus the proc (no variance).
        result.TotalNonCritical.Should().BeApproximately(500d + expectedProc, 1e-6);
        result.TotalCritical.Should().BeApproximately(1000d + expectedProc, 1e-6);
        result.TotalAverage.Should().BeApproximately(1000d + expectedProc, 1e-6);
    }

    [Fact]
    public void CalculateRotation_WithDuration_ComputesDps()
    {
        var rotation = new Rotation
        {
            Name = "10s rotation",
            DurationSeconds = 10,
            Steps = [new RotationStep { Name = "Hit", Hit = SimpleHit(1.0, 1000) }],
        };

        RotationResult result = Calculator.CalculateRotation(rotation);

        result.Dps.Should().BeApproximately(50d, 1e-6); // 500 average / 10s
    }

    [Fact]
    public void CalculateRotation_WithoutDuration_DpsIsZero()
    {
        var rotation = new Rotation
        {
            Name = "No duration",
            Steps = [new RotationStep { Name = "Hit", Hit = SimpleHit(1.0, 1000) }],
        };

        Calculator.CalculateRotation(rotation).Dps.Should().Be(0d);
    }

    [Fact]
    public void CalculateRotation_EmptySteps_ReturnsZeroTotals()
    {
        var rotation = new Rotation { Name = "Empty", Steps = [] };

        RotationResult result = Calculator.CalculateRotation(rotation);

        result.Steps.Should().BeEmpty();
        result.TotalNonCritical.Should().Be(0d);
        result.TotalCritical.Should().Be(0d);
        result.TotalAverage.Should().Be(0d);
    }

    [Fact]
    public void CalculateRotation_StepMissingHitAndTransformative_Throws()
    {
        var rotation = new Rotation
        {
            Name = "Invalid",
            Steps = [new RotationStep { Name = "Broken" }],
        };

        Action act = () => Calculator.CalculateRotation(rotation);

        act.Should().Throw<ArgumentException>().WithMessage("*Broken*");
    }
}
