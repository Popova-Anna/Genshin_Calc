using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Accounts.Commands.AnalyzeAccount;
using GenshinAccountAnalyzer.Domain.Analysis;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.GenerateReport;

/// <summary>
/// Handles <see cref="GenerateReportCommand"/> by analyzing the account (reusing the analyze pipeline)
/// and rendering the result as HTML.
/// </summary>
public sealed class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, string>
{
    private readonly ISender _sender;
    private readonly IReportGenerator _reportGenerator;

    /// <summary>Initializes the handler.</summary>
    /// <param name="sender">MediatR sender used to run the analyze command.</param>
    /// <param name="reportGenerator">The report generator.</param>
    public GenerateReportCommandHandler(ISender sender, IReportGenerator reportGenerator)
    {
        _sender = sender;
        _reportGenerator = reportGenerator;
    }

    /// <summary>Analyzes the account and renders the HTML report.</summary>
    /// <param name="request">The report command.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The rendered HTML document.</returns>
    public async Task<string> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        AccountAnalysis analysis = await _sender
            .Send(new AnalyzeAccountCommand(request.Content, request.Source), cancellationToken)
            .ConfigureAwait(false);

        return _reportGenerator.GenerateHtml(analysis);
    }
}
