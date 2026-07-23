using GenshinAccountAnalyzer.Application.Accounts.Commands.AnalyzeAccount;
using GenshinAccountAnalyzer.Application.Accounts.Commands.GenerateReport;
using GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GenshinAccountAnalyzer.Api.Controllers;

/// <summary>
/// Endpoints for importing and working with Genshin accounts.
/// </summary>
[ApiController]
[Route("api/account")]
public sealed class AccountController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>Initializes the controller.</summary>
    /// <param name="sender">The MediatR sender used to dispatch commands.</param>
    public AccountController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Imports a raw account export (default source: Enka.Network) and returns the internal account model.
    /// </summary>
    /// <param name="source">The export source. Defaults to <see cref="ImportSource.Enka"/>.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The imported <see cref="Account"/>.</returns>
    /// <response code="200">The account was imported successfully.</response>
    /// <response code="400">The content was invalid or could not be parsed.</response>
    [HttpPost("import")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Account>> Import(
        [FromQuery] ImportSource source = ImportSource.Enka,
        CancellationToken cancellationToken = default)
    {
        Account account = await _sender.Send(new ImportAccountCommand(Request.Body, source), cancellationToken);
        return Ok(account);
    }

    /// <summary>
    /// Imports a raw account export and analyzes every character (ratings, crit/ER/EM balance, efficiency).
    /// </summary>
    /// <param name="source">The export source. Defaults to <see cref="ImportSource.Enka"/>.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The <see cref="AccountAnalysis"/>.</returns>
    /// <response code="200">The account was imported and analyzed successfully.</response>
    /// <response code="400">The content was invalid or could not be parsed.</response>
    [HttpPost("analyze")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AccountAnalysis), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountAnalysis>> Analyze(
        [FromQuery] ImportSource source = ImportSource.Enka,
        CancellationToken cancellationToken = default)
    {
        AccountAnalysis analysis = await _sender.Send(
            new AnalyzeAccountCommand(Request.Body, source), cancellationToken);
        return Ok(analysis);
    }

    /// <summary>
    /// Imports and analyzes a raw account export and returns a self-contained HTML report.
    /// </summary>
    /// <param name="source">The export source. Defaults to <see cref="ImportSource.Enka"/>.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>An HTML document.</returns>
    /// <response code="200">The report was generated.</response>
    /// <response code="400">The content was invalid or could not be parsed.</response>
    [HttpPost("report")]
    [Consumes("application/json")]
    [Produces("text/html")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ContentResult> Report(
        [FromQuery] ImportSource source = ImportSource.Enka,
        CancellationToken cancellationToken = default)
    {
        string html = await _sender.Send(new GenerateReportCommand(Request.Body, source), cancellationToken);
        return Content(html, "text/html");
    }
}
