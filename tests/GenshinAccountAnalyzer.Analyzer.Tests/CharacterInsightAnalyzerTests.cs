using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer.Tests.Support;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests;

public sealed class CharacterInsightAnalyzerTests
{
    private static readonly CharacterInsightAnalyzer Analyzer = new();

    private static CharacterAnalysis Metrics(
        int level = 90,
        double buildScore = 85,
        double efficiency = 95,
        TalentLevels? talents = null,
        CritBalance? crit = null,
        double energyRecharge = 1.3,
        WeaponAnalysis? weapon = null,
        IReadOnlyList<ArtifactAnalysis>? artifacts = null) => new()
    {
        CharacterId = 1,
        Name = "Test",
        Level = level,
        MaxLevel = 90,
        ConstellationLevel = 0,
        Talents = talents,
        TalentRating = new Rating(100, RatingTier.SS),
        WeaponRating = new Rating(100, RatingTier.SS),
        ArtifactRating = new Rating(100, RatingTier.SS),
        BuildRating = new Rating(buildScore, RatingTier.S),
        OverallScore = buildScore,
        CritBalance = crit ?? new CritBalance(65, 130, 260, 2.0, 100, true),
        EnergyRecharge = energyRecharge,
        ElementalMastery = 100,
        Efficiency = efficiency,
        Artifacts = artifacts ?? [],
        Weapon = weapon,
    };

    private static WeaponAnalysis Weapon(double dpsLoss, string bisName = "Best Weapon") => new()
    {
        CharacterId = 1,
        WeaponType = WeaponType.Sword,
        Equipped = new WeaponOption(1, "Equipped", 5, 90, 100 - dpsLoss),
        Recommendations = [new WeaponOption(2, bisName, 5, 100, 100)],
        DpsLossVsBis = dpsLoss,
        ProfileName = "Test",
    };

    private static ArtifactAnalysis Artifact(double efficiency, double deadRolls) => new()
    {
        ArtifactId = 1,
        Slot = ArtifactSlot.Sands,
        SetName = "Set",
        Level = 20,
        Rarity = 5,
        CritValue = 0,
        RollValue = 700,
        RollCount = 9,
        Efficiency = efficiency,
        DeadRolls = deadRolls,
        Substats = [],
        ProfileName = "Test",
    };

    [Fact]
    public void Analyze_StrongBuild_ProducesStrengths()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword);
        CharacterAnalysis metrics = Metrics(
            buildScore: 92,
            efficiency: 98,
            talents: new TalentLevels(10, 10, 10),
            weapon: Weapon(dpsLoss: 2));

        CharacterInsights insights = Analyzer.Analyze(character, metrics);

        insights.Strengths.Should().Contain(s => s.Contains("Strong overall build"));
        insights.Strengths.Should().Contain(s => s.Contains("Maxed talents"));
        insights.Strengths.Should().Contain("Near-best-in-slot weapon");
        insights.BestWeapon!.Value.Name.Should().Be("Best Weapon");
    }

    [Fact]
    public void Analyze_WeakWeapon_RecommendsUpgradeWithHighPriority()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword);
        CharacterAnalysis metrics = Metrics(weapon: Weapon(dpsLoss: 30, bisName: "Mistsplitter Reforged"));

        CharacterInsights insights = Analyzer.Analyze(character, metrics);

        insights.Weaknesses.Should().Contain(w => w.Contains("below best-in-slot"));
        Recommendation upgrade = insights.Recommendations.First(r => r.Category == "weapon");
        upgrade.Priority.Should().Be(RecommendationPriority.High);
        upgrade.Detail.Should().Contain("Mistsplitter Reforged");
    }

    [Fact]
    public void Analyze_LowEnergyRecharge_IsFlagged()
    {
        CharacterAnalysis metrics = Metrics(energyRecharge: 0.9);

        CharacterInsights insights = Analyzer.Analyze(Build.Character(), metrics);

        insights.Weaknesses.Should().Contain(w => w.Contains("Low Energy Recharge"));
        insights.Recommendations.Should().Contain(r => r.Category == "energy");
    }

    [Fact]
    public void Analyze_InvestedButImbalancedCrit_IsFlagged()
    {
        CharacterAnalysis metrics = Metrics(
            crit: new CritBalance(70, 90, 230, 1.29, 60, IsBalanced: false));

        CharacterInsights insights = Analyzer.Analyze(Build.Character(), metrics);

        insights.Weaknesses.Should().Contain(w => w.Contains("Crit ratio off"));
        insights.Recommendations.Should().Contain(r => r.Category == "crit");
    }

    [Fact]
    public void Analyze_LowCritInvestment_DoesNotFlagCrit()
    {
        // CV below the investment floor: an EM/support build should not be penalised on crit balance.
        CharacterAnalysis metrics = Metrics(
            crit: new CritBalance(5, 50, 60, 10, 0, IsBalanced: false));

        CharacterInsights insights = Analyzer.Analyze(Build.Character(), metrics);

        insights.Weaknesses.Should().NotContain(w => w.Contains("Crit ratio"));
    }

    [Fact]
    public void Analyze_DeadRolls_AreFlagged()
    {
        CharacterAnalysis metrics = Metrics(artifacts: [Artifact(efficiency: 85, deadRolls: 2.5)]);

        CharacterInsights insights = Analyzer.Analyze(Build.Character(), metrics);

        insights.Weaknesses.Should().Contain(w => w.Contains("Wasted artifact rolls"));
    }

    [Fact]
    public void Analyze_SuboptimalGoblet_IsFlaggedAndRecommended()
    {
        Character character = BuildWithGoblet(ElementType.Pyro, StatType.HpPercent);

        CharacterInsights insights = Analyzer.Analyze(character, Metrics());

        insights.Weaknesses.Should().Contain(w => w.Contains("Goblet main stat"));
        insights.Recommendations.Should().Contain(r => r.Category == "artifacts" && r.Title.Contains("goblet"));
        insights.BestArtifacts.MainStats[ArtifactSlot.Goblet].Should().Be(StatType.PyroDamageBonus);
    }

    [Fact]
    public void Analyze_CorrectGoblet_IsNotFlagged()
    {
        Character character = BuildWithGoblet(ElementType.Pyro, StatType.PyroDamageBonus);

        CharacterInsights insights = Analyzer.Analyze(character, Metrics());

        insights.Weaknesses.Should().NotContain(w => w.Contains("Goblet"));
    }

    [Fact]
    public void Analyze_DetectsCurrentSets()
    {
        var artifacts = new List<Artifact>
        {
            GobletlessArtifact(ArtifactSlot.Flower, 15001),
            GobletlessArtifact(ArtifactSlot.Plume, 15001),
            GobletlessArtifact(ArtifactSlot.Sands, 15001),
            GobletlessArtifact(ArtifactSlot.Goblet, 15001),
            GobletlessArtifact(ArtifactSlot.Circlet, 15002),
        };
        Character character = Build.Character(artifacts: artifacts);

        CharacterInsights insights = Analyzer.Analyze(character, Metrics());

        EquippedSet fourPiece = insights.BestArtifacts.CurrentSets.First();
        fourPiece.SetId.Should().Be(15001);
        fourPiece.PieceCount.Should().Be(4);
    }

    [Fact]
    public void Analyze_UnderLeveled_RecommendsLevelWithHighPriority()
    {
        CharacterInsights insights = Analyzer.Analyze(Build.Character(level: 70), Metrics(level: 70));

        insights.Weaknesses.Should().Contain(w => w.Contains("Below max level"));
        insights.Recommendations.First(r => r.Category == "level").Priority.Should().Be(RecommendationPriority.High);
    }

    private static Character BuildWithGoblet(ElementType element, StatType gobletMain)
    {
        var goblet = new Artifact
        {
            Id = 1,
            SetId = 15001,
            SetName = "Set",
            Slot = ArtifactSlot.Goblet,
            Rarity = 5,
            Level = 20,
            MainStat = new Stat(gobletMain, 46.6),
            SubStats = [],
        };
        return new Character
        {
            Id = 1,
            Name = "Test",
            Element = element,
            Level = 90,
            Talents = [],
            Artifacts = [goblet],
            Stats = StatSheet.Empty,
        };
    }

    private static Artifact GobletlessArtifact(ArtifactSlot slot, int setId) => new()
    {
        Id = (int)slot,
        SetId = setId,
        SetName = $"Set {setId}",
        Slot = slot,
        Rarity = 5,
        Level = 20,
        MainStat = new Stat(StatType.Atk, 311),
        SubStats = [],
    };
}
