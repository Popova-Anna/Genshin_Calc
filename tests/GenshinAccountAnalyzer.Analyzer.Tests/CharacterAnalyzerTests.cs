using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer.Tests.Support;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests;

public sealed class CharacterAnalyzerTests
{
    private static ICharacterAnalyzer CreateAnalyzer(IGameMetadataProvider? metadata = null) =>
        new CharacterAnalyzer(metadata ?? new FakeMetadata(), new ArtifactAnalyzer());

    [Fact]
    public void Analyze_FullyBuiltCharacter_ScoresTopTiers()
    {
        var metadata = new FakeMetadata().WithCharacter(10000047, talents: [1, 2, 3]);
        Character character = Build.Character(
            id: 10000047,
            level: 90,
            talents: [new Talent(1, 10), new Talent(2, 10), new Talent(3, 10)],
            weapon: Build.Weapon(level: 90, rarity: 5, refinement: 5),
            artifacts: Build.FullArtifactSet(level: 20, rarity: 5, critRateSub: 0.10, critDamageSub: 0.20),
            stats: Build.Stats(
                (StatType.CritRate, 0.5), (StatType.CritDamage, 1.0),
                (StatType.EnergyRecharge, 1.2), (StatType.ElementalMastery, 100)));

        CharacterAnalysis analysis = CreateAnalyzer(metadata).Analyze(character);

        analysis.TalentRating.Tier.Should().Be(RatingTier.SS);
        analysis.WeaponRating.Tier.Should().Be(RatingTier.SS);
        analysis.ArtifactRating.Tier.Should().Be(RatingTier.SS);
        analysis.BuildRating.Tier.Should().Be(RatingTier.SS);
        analysis.OverallScore.Should().BeApproximately(100d, 0.001);
        analysis.Efficiency.Should().BeApproximately(100d, 0.001);
    }

    [Fact]
    public void Analyze_IdentifiesMainTalents_FromMetadata()
    {
        var metadata = new FakeMetadata().WithCharacter(10000047, talents: [10471, 10472, 10475]);
        Character character = Build.Character(
            id: 10000047,
            talents: [new Talent(10471, 8), new Talent(10472, 9), new Talent(10475, 10)]);

        CharacterAnalysis analysis = CreateAnalyzer(metadata).Analyze(character);

        analysis.Talents.Should().Be(new TalentLevels(8, 9, 10));
    }

    [Fact]
    public void Analyze_WithoutMetadata_LeavesTalentsUnidentifiedButStillRates()
    {
        Character character = Build.Character(
            talents: [new Talent(1, 10), new Talent(2, 10)]);

        CharacterAnalysis analysis = CreateAnalyzer().Analyze(character);

        analysis.Talents.Should().BeNull();
        analysis.TalentRating.Score.Should().BeApproximately(100d, 0.001, "fallback averages all reported talents");
    }

    [Fact]
    public void Analyze_IdealCritRatio_IsBalanced()
    {
        Character character = Build.Character(
            stats: Build.Stats((StatType.CritRate, 0.6), (StatType.CritDamage, 1.2)));

        CritBalance crit = CreateAnalyzer().Analyze(character).CritBalance;

        crit.CritRate.Should().BeApproximately(60d, 0.001);
        crit.CritDamage.Should().BeApproximately(120d, 0.001);
        crit.CritValue.Should().BeApproximately(240d, 0.001);
        crit.Ratio.Should().BeApproximately(2.0, 0.001);
        crit.IsBalanced.Should().BeTrue();
        crit.BalanceScore.Should().BeApproximately(100d, 0.001);
    }

    [Fact]
    public void Analyze_ImbalancedCrit_IsNotBalanced()
    {
        // CR 50%, CD 60% -> ratio 1.2, well outside the tolerance band around 2.0.
        Character character = Build.Character(
            stats: Build.Stats((StatType.CritRate, 0.5), (StatType.CritDamage, 0.6)));

        CritBalance crit = CreateAnalyzer().Analyze(character).CritBalance;

        crit.Ratio.Should().BeApproximately(1.2, 0.001);
        crit.IsBalanced.Should().BeFalse();
        crit.BalanceScore.Should().BeLessThan(100d);
    }

    [Fact]
    public void Analyze_NegligibleCritRate_IsNotAssessable()
    {
        Character character = Build.Character(
            stats: Build.Stats((StatType.CritRate, 0.03), (StatType.CritDamage, 0.5)));

        CritBalance crit = CreateAnalyzer().Analyze(character).CritBalance;

        crit.IsBalanced.Should().BeFalse();
        crit.Ratio.Should().Be(0d);
        crit.BalanceScore.Should().Be(0d);
    }

    [Fact]
    public void Analyze_NoWeapon_WeaponRatingIsZero()
    {
        Character character = Build.Character(weapon: null);

        Rating weaponRating = CreateAnalyzer().Analyze(character).WeaponRating;

        weaponRating.Score.Should().Be(0d);
        weaponRating.Tier.Should().Be(RatingTier.F);
    }

    [Fact]
    public void Analyze_UnbuiltCharacter_HasLowEfficiency()
    {
        Character character = Build.Character(
            level: 1,
            talents: [new Talent(1, 1)],
            weapon: null,
            artifacts: []);

        CharacterAnalysis analysis = CreateAnalyzer().Analyze(character);

        analysis.Efficiency.Should().BeLessThan(20d);
        analysis.BuildRating.Tier.Should().Be(RatingTier.F);
    }

    [Fact]
    public void Analyze_ReportsEnergyRechargeAndMastery()
    {
        Character character = Build.Character(
            stats: Build.Stats((StatType.EnergyRecharge, 1.35), (StatType.ElementalMastery, 250)));

        CharacterAnalysis analysis = CreateAnalyzer().Analyze(character);

        analysis.EnergyRecharge.Should().BeApproximately(1.35, 0.001);
        analysis.ElementalMastery.Should().BeApproximately(250d, 0.001);
    }
}
