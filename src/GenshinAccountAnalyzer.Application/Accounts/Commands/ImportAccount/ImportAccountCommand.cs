using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;

/// <summary>
/// Command to import a raw account export into the internal <see cref="Account"/> model.
/// </summary>
/// <param name="Content">The raw export content stream.</param>
/// <param name="Source">The source the content originates from. Defaults to <see cref="ImportSource.Enka"/>.</param>
public sealed record ImportAccountCommand(Stream Content, ImportSource Source = ImportSource.Enka)
    : IRequest<Account>;
