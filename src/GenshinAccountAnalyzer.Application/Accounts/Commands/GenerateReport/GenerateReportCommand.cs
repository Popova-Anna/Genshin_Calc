using GenshinAccountAnalyzer.Domain.Enums;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.GenerateReport;

/// <summary>
/// Command to import and analyze an account export and render it as an HTML report.
/// </summary>
/// <param name="Content">The raw export content stream.</param>
/// <param name="Source">The source the content originates from. Defaults to <see cref="ImportSource.Enka"/>.</param>
public sealed record GenerateReportCommand(Stream Content, ImportSource Source = ImportSource.Enka)
    : IRequest<string>;
