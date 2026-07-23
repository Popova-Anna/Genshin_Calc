using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;
using GenshinAccountAnalyzer.Parser.Enka;
using GenshinAccountAnalyzer.Parser.Metadata;
using GenshinAccountAnalyzer.Parser.Tests.TestData;

namespace GenshinAccountAnalyzer.Parser.Tests.Analysis;

/// <summary>End-to-end: import the real sample and analyze it with the default (embedded) metadata.</summary>
public sealed class AnalyzeSampleTests
{
    private static async Task<(Account Account, CharacterAnalyzer Analyzer)> ImportAsync()
    {
        var metadata = EmbeddedGameMetadataProvider.CreateDefault();
        await using FileStream sample = SampleData.OpenEnkaSample();
        Account account = await new EnkaImporter(metadata).ImportAsync(sample, CancellationToken.None);
        return (account, new CharacterAnalyzer(
            metadata, new ArtifactAnalyzer(), new WeaponAnalyzer(metadata), new CharacterInsightAnalyzer()));
    }

    [Fact]
    public async Task Analyze_SampleKazuha_ProducesIdentifiedTalentsAndHighEfficiency()
    {
        (Account account, CharacterAnalyzer analyzer) = await ImportAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        CharacterAnalysis analysis = analyzer.Analyze(kazuha);

        analysis.Name.Should().Be("Kaedehara Kazuha");
        analysis.Talents.Should().Be(new TalentLevels(10, 10, 10));
        analysis.TalentRating.Tier.Should().Be(RatingTier.SS);
        analysis.Efficiency.Should().BeApproximately(100d, 0.5, "the character is level 90 with maxed talents, weapon and +20 artifacts");
        analysis.OverallScore.Should().BeGreaterThan(70d);
    }

    [Fact]
    public async Task Analyze_SampleKazuha_IncludesArtifactAnalyses()
    {
        (Account account, CharacterAnalyzer analyzer) = await ImportAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        CharacterAnalysis analysis = analyzer.Analyze(kazuha);

        analysis.Artifacts.Should().HaveCount(5);
        ArtifactAnalysis flower = analysis.Artifacts.Single(a => a.Slot == ArtifactSlot.Flower);
        flower.RollCount.Should().Be(8);
        flower.CritValue.Should().BeApproximately(21.8, 0.1);
        flower.Efficiency.Should().BeInRange(70d, 100d);
    }

    [Fact]
    public async Task Analyze_SampleKazuha_RanksWeaponsAndResolvesName()
    {
        (Account account, CharacterAnalyzer analyzer) = await ImportAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        // Weapon name is now resolved from the embedded catalog (no more placeholder).
        kazuha.Weapon!.Name.Should().Be("Freedom-Sworn");

        WeaponAnalysis weapon = analyzer.Analyze(kazuha).Weapon!;
        weapon.WeaponType.Should().Be(WeaponType.Sword);
        weapon.Equipped!.Value.Name.Should().Be("Freedom-Sworn");
        weapon.Recommendations.Should().NotBeEmpty();
        weapon.Recommendations[0].RelativeToBis.Should().BeApproximately(100d, 0.001);
        weapon.DpsLossVsBis.Should().BeInRange(0d, 100d);
    }

    [Fact]
    public async Task Analyze_SampleKazuha_ProducesInsights()
    {
        (Account account, CharacterAnalyzer analyzer) = await ImportAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        CharacterAnalysis analysis = analyzer.Analyze(kazuha);

        analysis.Strengths.Should().NotBeEmpty();
        analysis.BestArtifacts.Should().NotBeNull();
        analysis.BestArtifacts!.MainStats[ArtifactSlot.Goblet].Should().Be(StatType.AnemoDamageBonus);
        analysis.BestArtifacts.MainStats[ArtifactSlot.Flower].Should().Be(StatType.Hp);
        analysis.BestWeapon.Should().NotBeNull();
    }

    [Fact]
    public async Task FindBestTeams_OnSampleRoster_ProducesRankedFullTeams()
    {
        (Account account, CharacterAnalyzer analyzer) = await ImportAsync();
        Dictionary<int, double> buildScores = account.Characters
            .ToDictionary(c => c.Id, c => analyzer.Analyze(c).OverallScore);

        IReadOnlyList<TeamAnalysis> teams = new TeamAnalyzer().FindBestTeams(account.Characters, buildScores, 5);

        teams.Should().HaveCount(5);
        teams.Should().BeInDescendingOrder(t => t.Score);
        teams[0].Members.Should().HaveCount(4);
        teams[0].Members[0].Role.Should().Be("Carry");
        teams[0].Reasons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Analyze_EveryCharacter_ProducesValidScores()
    {
        (Account account, CharacterAnalyzer analyzer) = await ImportAsync();

        foreach (Character character in account.Characters)
        {
            CharacterAnalysis analysis = analyzer.Analyze(character);
            analysis.OverallScore.Should().BeInRange(0d, 100d);
            analysis.Efficiency.Should().BeInRange(0d, 100d);
            analysis.BuildRating.Score.Should().BeInRange(0d, 100d);
        }
    }
}
