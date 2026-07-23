using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer;
using GenshinAccountAnalyzer.Domain.Analysis;
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
        return (account, new CharacterAnalyzer(metadata));
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
