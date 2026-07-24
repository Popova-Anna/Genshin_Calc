using FluentAssertions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests;

public sealed class BuildOptimizerTests
{
    private static readonly BuildOptimizer Optimizer = new();

    private static Artifact Piece(ArtifactSlot slot, StatType main) => new()
    {
        Id = (int)slot,
        SetId = 15001,
        SetName = "Set",
        Slot = slot,
        Rarity = 5,
        Level = 20,
        MainStat = new Stat(main, 46.6),
        SubStats = [],
    };

    private static CharacterAnalysis AnalysisFor(Character character, ElementType element) => new()
    {
        CharacterId = character.Id,
        Name = "Test",
        Element = element,
        Level = 90,
        MaxLevel = 90,
        ConstellationLevel = 0,
        TalentRating = new Rating(0, RatingTier.F),
        WeaponRating = new Rating(0, RatingTier.F),
        ArtifactRating = new Rating(0, RatingTier.F),
        BuildRating = new Rating(0, RatingTier.F),
        OverallScore = 0,
        CritBalance = default,
        EnergyRecharge = 1,
        ElementalMastery = 0,
        Efficiency = 0,
        BestArtifacts = new ArtifactRecommendation
        {
            MainStats = new Dictionary<ArtifactSlot, StatType>
            {
                [ArtifactSlot.Flower] = StatType.Hp,
                [ArtifactSlot.Plume] = StatType.Atk,
                [ArtifactSlot.Sands] = StatType.AtkPercent,
                [ArtifactSlot.Goblet] = StatType.PyroDamageBonus,
                [ArtifactSlot.Circlet] = StatType.CritRate,
            },
            Substats = [],
            CurrentSets = [],
            Notes = "test",
        },
    };

    private static Character CharacterWith(params Artifact[] artifacts) => new()
    {
        Id = 1,
        Name = "Test",
        Element = ElementType.Pyro,
        Level = 90,
        Talents = [],
        Artifacts = artifacts,
        Stats = Domain.Common.StatSheet.Empty,
    };

    [Fact]
    public void Optimize_AllOptimalGear_ScoresFull()
    {
        Character character = CharacterWith(
            Piece(ArtifactSlot.Flower, StatType.Hp),
            Piece(ArtifactSlot.Plume, StatType.Atk),
            Piece(ArtifactSlot.Sands, StatType.AtkPercent),
            Piece(ArtifactSlot.Goblet, StatType.PyroDamageBonus),
            Piece(ArtifactSlot.Circlet, StatType.CritDamage));

        BuildOptimization result = Optimizer.Optimize(character, AnalysisFor(character, ElementType.Pyro));

        result.OptimizationScore.Should().Be(100d);
        result.Notes.Should().BeEmpty();
        result.Slots.Should().OnlyContain(s => s.IsOptimal);
    }

    [Fact]
    public void Optimize_WrongGoblet_IsFlagged()
    {
        Character character = CharacterWith(
            Piece(ArtifactSlot.Flower, StatType.Hp),
            Piece(ArtifactSlot.Plume, StatType.Atk),
            Piece(ArtifactSlot.Sands, StatType.AtkPercent),
            Piece(ArtifactSlot.Goblet, StatType.HpPercent),   // wrong: defensive main on goblet
            Piece(ArtifactSlot.Circlet, StatType.CritRate));

        BuildOptimization result = Optimizer.Optimize(character, AnalysisFor(character, ElementType.Pyro));

        result.OptimizationScore.Should().BeApproximately(80d, 0.001);
        SlotOptimization goblet = result.Slots.Single(s => s.Slot == ArtifactSlot.Goblet);
        goblet.IsOptimal.Should().BeFalse();
        goblet.RecommendedMain.Should().Be(StatType.PyroDamageBonus);
        result.Notes.Should().ContainSingle();
        result.Notes[0].En.Should().Contain("Goblet");
        result.Notes[0].Ru.Should().Contain("Goblet");
    }

    [Fact]
    public void Optimize_MissingSlot_IsFlaggedAsNotOptimal()
    {
        Character character = CharacterWith(
            Piece(ArtifactSlot.Flower, StatType.Hp),
            Piece(ArtifactSlot.Plume, StatType.Atk));

        BuildOptimization result = Optimizer.Optimize(character, AnalysisFor(character, ElementType.Pyro));

        result.OptimizationScore.Should().BeApproximately(40d, 0.001);
        result.Slots.Count(s => !s.IsOptimal).Should().Be(3);
    }

    [Fact]
    public void Optimize_EmMainStats_AreAccepted()
    {
        // EM sands and EM goblet are legitimate for reaction builds and must not be flagged.
        Character character = CharacterWith(
            Piece(ArtifactSlot.Flower, StatType.Hp),
            Piece(ArtifactSlot.Plume, StatType.Atk),
            Piece(ArtifactSlot.Sands, StatType.ElementalMastery),
            Piece(ArtifactSlot.Goblet, StatType.ElementalMastery),
            Piece(ArtifactSlot.Circlet, StatType.CritRate));

        BuildOptimization result = Optimizer.Optimize(character, AnalysisFor(character, ElementType.Pyro));

        result.OptimizationScore.Should().Be(100d);
    }
}
