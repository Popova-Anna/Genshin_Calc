using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.AnalyzeAccount;

/// <summary>
/// Command to import a raw account export and analyze every character it contains.
/// </summary>
/// <param name="Content">The raw export content stream.</param>
/// <param name="Source">The source the content originates from. Defaults to <see cref="ImportSource.Enka"/>.</param>
public sealed record AnalyzeAccountCommand(Stream Content, ImportSource Source = ImportSource.Enka)
    : IRequest<AccountAnalysis>;
