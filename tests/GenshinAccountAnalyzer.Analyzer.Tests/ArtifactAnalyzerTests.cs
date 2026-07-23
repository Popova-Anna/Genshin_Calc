using FluentAssertions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests;

public sealed class ArtifactAnalyzerTests
{
    private static Artifact Artifact(int rollCount, params Stat[] subs) => new()
    {
        Id = 1,
        SetId = 15002,
        SetName = "Test Set",
        Slot = ArtifactSlot.Flower,
        Rarity = 5,
        Level = 20,
        MainStat = new Stat(StatType.Hp, 4780),
        SubStats = subs,
        RollCount = rollCount,
    };

    [Fact]
    public void Analyze_ComputesCritValueFromSubstats()
    {
        Artifact artifact = Artifact(
            rollCount: 4,
            new Stat(StatType.CritRate, 0.05),
            new Stat(StatType.CritDamage, 0.10));

        ArtifactAnalysis analysis = new ArtifactAnalyzer().Analyze(artifact);

        // 2 * 5% + 10% = 20.
        analysis.CritValue.Should().BeApproximately(20d, 0.001);
    }

    [Fact]
    public void Analyze_ComputesRollValueAndEfficiency()
    {
        // Sample flower: CD 21.8%, ER 10.4%, ATK 37, EM 16 across 8 rolls.
        Artifact artifact = Artifact(
            rollCount: 8,
            new Stat(StatType.CritDamage, 0.218),
            new Stat(StatType.EnergyRecharge, 0.104),
            new Stat(StatType.Atk, 37),
            new Stat(StatType.ElementalMastery, 16));

        ArtifactAnalysis analysis = new ArtifactAnalyzer().Analyze(artifact);

        analysis.RollCount.Should().Be(8);
        analysis.RollValue.Should().BeApproximately(699.9, 0.5);
        analysis.Efficiency.Should().BeApproximately(87.5, 0.2);
    }

    [Fact]
    public void Analyze_FlagsDeadRolls_ForUselessSubstats()
    {
        // Flat DEF has weight 0 in the default profile; one max DEF roll is 23.15.
        Artifact artifact = Artifact(
            rollCount: 4,
            new Stat(StatType.CritRate, 0.0389),
            new Stat(StatType.Def, 23.15));

        ArtifactAnalysis analysis = new ArtifactAnalyzer().Analyze(artifact);

        SubstatAnalysis def = analysis.Substats.Single(s => s.Type == StatType.Def);
        def.IsDead.Should().BeTrue();
        def.Weight.Should().Be(0d);

        SubstatAnalysis crit = analysis.Substats.Single(s => s.Type == StatType.CritRate);
        crit.IsDead.Should().BeFalse();

        analysis.DeadRolls.Should().BeApproximately(1.0, 0.01, "the flat DEF substat is one full dead roll");
    }

    [Fact]
    public void Analyze_ReportsSubstatUsefulness()
    {
        Artifact artifact = Artifact(
            rollCount: 4,
            new Stat(StatType.CritDamage, 0.0777));

        ArtifactAnalysis analysis = new ArtifactAnalyzer().Analyze(artifact);

        SubstatAnalysis crit = analysis.Substats.Single();
        crit.RollValue.Should().BeApproximately(1.0, 0.001);
        crit.Weight.Should().Be(1.0d);
        crit.Usefulness.Should().BeApproximately(1.0, 0.001);
    }

    [Fact]
    public void Analyze_WithoutRollCount_EstimatesFromLevel()
    {
        Artifact artifact = Artifact(
            rollCount: 0,
            new Stat(StatType.CritDamage, 0.0777));

        ArtifactAnalysis analysis = new ArtifactAnalyzer().Analyze(artifact);

        // 4 initial substats + level 20 / 4 = 9.
        analysis.RollCount.Should().Be(9);
    }

    [Fact]
    public void Analyze_RespectsCustomProfile()
    {
        var profile = new SubstatWeightProfile(
            "Off Stat",
            new Dictionary<StatType, double> { [StatType.CritDamage] = 0d });
        Artifact artifact = Artifact(rollCount: 4, new Stat(StatType.CritDamage, 0.0777));

        ArtifactAnalysis analysis = new ArtifactAnalyzer().Analyze(artifact, profile);

        analysis.ProfileName.Should().Be("Off Stat");
        analysis.Substats.Single().IsDead.Should().BeTrue();
        analysis.DeadRolls.Should().BeApproximately(1.0, 0.001);
    }
}
