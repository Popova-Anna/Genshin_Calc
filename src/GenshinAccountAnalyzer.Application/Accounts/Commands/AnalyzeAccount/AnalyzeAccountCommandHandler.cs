using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.AnalyzeAccount;

/// <summary>
/// Handles <see cref="AnalyzeAccountCommand"/> by importing the account (reusing the import pipeline,
/// including its validation), analyzing every character and finding the best teams.
/// </summary>
public sealed class AnalyzeAccountCommandHandler : IRequestHandler<AnalyzeAccountCommand, AccountAnalysis>
{
    /// <summary>How many best teams to attach to each character.</summary>
    private const int TeamsPerCharacter = 3;

    private readonly ISender _sender;
    private readonly ICharacterAnalyzer _characterAnalyzer;
    private readonly ITeamAnalyzer _teamAnalyzer;

    /// <summary>Initializes the handler.</summary>
    /// <param name="sender">MediatR sender used to run the import command.</param>
    /// <param name="characterAnalyzer">The per-character analyzer.</param>
    /// <param name="teamAnalyzer">The team analyzer.</param>
    public AnalyzeAccountCommandHandler(
        ISender sender,
        ICharacterAnalyzer characterAnalyzer,
        ITeamAnalyzer teamAnalyzer)
    {
        _sender = sender;
        _characterAnalyzer = characterAnalyzer;
        _teamAnalyzer = teamAnalyzer;
    }

    /// <summary>Imports and analyzes the account, then finds the best teams.</summary>
    /// <param name="request">The analyze command.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The account analysis.</returns>
    public async Task<AccountAnalysis> Handle(AnalyzeAccountCommand request, CancellationToken cancellationToken)
    {
        Account account = await _sender
            .Send(new ImportAccountCommand(request.Content, request.Source), cancellationToken)
            .ConfigureAwait(false);

        List<CharacterAnalysis> analyses = account.Characters
            .Select(_characterAnalyzer.Analyze)
            .ToList();

        Dictionary<int, double> buildScores = analyses.ToDictionary(a => a.CharacterId, a => a.OverallScore);
        IReadOnlyList<TeamAnalysis> teams = _teamAnalyzer.FindBestTeams(
            account.Characters, buildScores, TeamScoreCount);

        // Attach each character's best featuring teams.
        List<CharacterAnalysis> enriched = analyses
            .Select(analysis => analysis with { BestTeams = TeamsFeaturing(teams, analysis.CharacterId) })
            .ToList();

        double averageBuildScore = analyses.Count == 0
            ? 0d
            : analyses.Average(analysis => analysis.OverallScore);

        return new AccountAnalysis
        {
            Uid = account.Uid,
            Characters = enriched,
            AverageBuildScore = averageBuildScore,
            Teams = teams,
        };
    }

    private static IReadOnlyList<TeamAnalysis> TeamsFeaturing(IReadOnlyList<TeamAnalysis> teams, int characterId) =>
        teams
            .Where(team => team.Members.Any(member => member.CharacterId == characterId))
            .Take(TeamsPerCharacter)
            .ToList();

    /// <summary>How many overall best teams to compute for the account.</summary>
    private const int TeamScoreCount = 8;
}
