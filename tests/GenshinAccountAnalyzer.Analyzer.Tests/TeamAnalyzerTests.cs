using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer.Tests.Support;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests;

public sealed class TeamAnalyzerTests
{
    private static readonly TeamAnalyzer Analyzer = new();

    private static Character Character(int id, ElementType element) =>
        Build.Character(id: id, name: $"C{id}", element: element);

    private static Dictionary<int, double> Scores(IEnumerable<Character> characters, double value = 80d) =>
        characters.ToDictionary(c => c.Id, _ => value);

    [Fact]
    public void FindBestTeams_FewerThanFullTeam_ReturnsEmpty()
    {
        List<Character> roster =
        [
            Character(1, ElementType.Pyro),
            Character(2, ElementType.Hydro),
            Character(3, ElementType.Anemo),
        ];

        Analyzer.FindBestTeams(roster, Scores(roster), 5).Should().BeEmpty();
    }

    [Fact]
    public void FindBestTeams_DetectsVaporizeCore()
    {
        List<Character> roster =
        [
            Character(1, ElementType.Pyro),
            Character(2, ElementType.Hydro),
            Character(3, ElementType.Anemo),
            Character(4, ElementType.Geo),
        ];

        IReadOnlyList<TeamAnalysis> teams = Analyzer.FindBestTeams(roster, Scores(roster), 5);

        teams.Should().ContainSingle();
        teams[0].ReactionCore.Should().Be("Vaporize");
    }

    [Fact]
    public void FindBestTeams_MonoElement_IsCohesiveWithResonance()
    {
        List<Character> roster =
        [
            Character(1, ElementType.Pyro),
            Character(2, ElementType.Pyro),
            Character(3, ElementType.Pyro),
            Character(4, ElementType.Pyro),
        ];

        TeamAnalysis team = Analyzer.FindBestTeams(roster, Scores(roster), 5)[0];

        team.ReactionCore.Should().Be("Mono Pyro");
        team.Resonances.Should().ContainSingle(r => r.Element == ElementType.Pyro);
    }

    [Fact]
    public void FindBestTeams_DetectsMultipleResonances()
    {
        List<Character> roster =
        [
            Character(1, ElementType.Pyro),
            Character(2, ElementType.Pyro),
            Character(3, ElementType.Cryo),
            Character(4, ElementType.Cryo),
        ];

        TeamAnalysis team = Analyzer.FindBestTeams(roster, Scores(roster), 5)[0];

        team.Resonances.Select(r => r.Element).Should().BeEquivalentTo([ElementType.Pyro, ElementType.Cryo]);
    }

    [Fact]
    public void FindBestTeams_RanksHigherSynergyAndPowerFirst()
    {
        // Strong, well-built vaporize core vs a weak mono-off team.
        List<Character> roster =
        [
            Character(1, ElementType.Pyro),
            Character(2, ElementType.Hydro),
            Character(3, ElementType.Anemo),
            Character(4, ElementType.Dendro),
            Character(5, ElementType.Geo),
            Character(6, ElementType.Geo),
        ];
        var scores = new Dictionary<int, double>
        {
            [1] = 95, [2] = 95, [3] = 90, [4] = 90, [5] = 30, [6] = 30,
        };

        IReadOnlyList<TeamAnalysis> teams = Analyzer.FindBestTeams(roster, scores, 3);

        teams.Should().BeInDescendingOrder(t => t.Score);
        teams[0].Members.Should().NotContain(m => m.CharacterId == 5 || m.CharacterId == 6);
    }

    [Fact]
    public void FindBestTeams_AssignsCarryToHighestBuildScore()
    {
        List<Character> roster =
        [
            Character(1, ElementType.Pyro),
            Character(2, ElementType.Hydro),
            Character(3, ElementType.Anemo),
            Character(4, ElementType.Cryo),
        ];
        var scores = new Dictionary<int, double> { [1] = 95, [2] = 70, [3] = 60, [4] = 50 };

        TeamAnalysis team = Analyzer.FindBestTeams(roster, scores, 5)[0];

        team.Members[0].Role.Should().Be("Carry");
        team.Members[0].CharacterId.Should().Be(1);
    }

    [Fact]
    public void FindBestTeams_LimitsToRequestedCount()
    {
        List<Character> roster = Enumerable.Range(1, 8)
            .Select(i => Character(i, ElementType.Pyro))
            .ToList();

        Analyzer.FindBestTeams(roster, Scores(roster), 2).Should().HaveCount(2);
    }
}
