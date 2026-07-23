using FluentAssertions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Report;

namespace GenshinAccountAnalyzer.Report.Tests;

public sealed class HtmlReportGeneratorTests
{
    private static CharacterAnalysis Character(string name, double score = 85, ElementType element = ElementType.Pyro) => new()
    {
        CharacterId = 1,
        Name = name,
        Element = element,
        Level = 90,
        MaxLevel = 90,
        ConstellationLevel = 2,
        TalentRating = new Rating(90, RatingTier.S),
        WeaponRating = new Rating(80, RatingTier.A),
        ArtifactRating = new Rating(75, RatingTier.A),
        BuildRating = new Rating(score, RatingTier.S),
        OverallScore = score,
        CritBalance = new CritBalance(65, 130, 260, 2.0, 100, true),
        EnergyRecharge = 1.2,
        ElementalMastery = 100,
        Efficiency = 95,
        Strengths = ["Strong overall build"],
        Weaknesses = ["Weapon well below best-in-slot"],
        Recommendations = [new Recommendation("weapon", "Upgrade weapon", "Consider Mistsplitter.", RecommendationPriority.High)],
    };

    private static AccountAnalysis Account(params CharacterAnalysis[] characters) => new()
    {
        Uid = "627846506",
        Characters = characters,
        AverageBuildScore = characters.Length == 0 ? 0 : characters.Average(c => c.OverallScore),
        Teams =
        [
            new TeamAnalysis
            {
                Members = [new TeamMember(1, characters.FirstOrDefault()?.Name ?? "X", ElementType.Pyro, 85, "Carry")],
                Score = 88,
                ReactionCore = "Vaporize",
                Resonances = [new TeamResonance(ElementType.Pyro, "Fervent Flames", "+25% ATK")],
                Reasons = ["Reaction core: Vaporize"],
            },
        ],
    };

    private readonly HtmlReportGenerator _generator = new();

    [Fact]
    public void GenerateHtml_ProducesCompleteDocument()
    {
        string html = _generator.GenerateHtml(Account(Character("Hu Tao")));

        html.Should().StartWith("<!DOCTYPE html>");
        html.Should().EndWith("</body></html>");
        html.Should().Contain("<style>").And.Contain("627846506");
    }

    [Fact]
    public void GenerateHtml_ContainsEverySection()
    {
        string html = _generator.GenerateHtml(Account(Character("Hu Tao")));

        foreach (string id in new[] { "home", "characters", "weapons", "artifacts", "teams", "statistics", "rating", "recommendations", "history" })
        {
            html.Should().Contain($"id=\"{id}\"");
        }
    }

    [Fact]
    public void GenerateHtml_RendersCharacterAndTeamData()
    {
        string html = _generator.GenerateHtml(Account(Character("Neuvillette")));

        html.Should().Contain("Neuvillette");
        html.Should().Contain("Vaporize");
        html.Should().Contain("Fervent Flames");
        html.Should().Contain("Upgrade weapon");
    }

    [Fact]
    public void GenerateHtml_EscapesHtmlInNames()
    {
        string html = _generator.GenerateHtml(Account(Character("<script>alert(1)</script>")));

        html.Should().NotContain("<script>alert(1)</script>");
        html.Should().Contain("&lt;script&gt;");
    }

    [Fact]
    public void GenerateHtml_EmptyAccount_StillRenders()
    {
        string html = _generator.GenerateHtml(Account());

        html.Should().StartWith("<!DOCTYPE html>");
        html.Should().Contain("id=\"recommendations\"");
    }
}
