using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.AnalyzeAccount;

/// <summary>
/// Handles <see cref="AnalyzeAccountCommand"/> by importing the account (reusing the import pipeline,
/// including its validation) and then analyzing every character.
/// </summary>
public sealed class AnalyzeAccountCommandHandler : IRequestHandler<AnalyzeAccountCommand, AccountAnalysis>
{
    private readonly ISender _sender;
    private readonly ICharacterAnalyzer _characterAnalyzer;

    /// <summary>Initializes the handler.</summary>
    /// <param name="sender">MediatR sender used to run the import command.</param>
    /// <param name="characterAnalyzer">The per-character analyzer.</param>
    public AnalyzeAccountCommandHandler(ISender sender, ICharacterAnalyzer characterAnalyzer)
    {
        _sender = sender;
        _characterAnalyzer = characterAnalyzer;
    }

    /// <summary>Imports and analyzes the account.</summary>
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

        double averageBuildScore = analyses.Count == 0
            ? 0d
            : analyses.Average(analysis => analysis.OverallScore);

        return new AccountAnalysis
        {
            Uid = account.Uid,
            Characters = analyses,
            AverageBuildScore = averageBuildScore,
        };
    }
}
