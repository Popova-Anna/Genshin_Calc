using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Exceptions;
using GenshinAccountAnalyzer.Domain.Models;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;

/// <summary>
/// Handles <see cref="ImportAccountCommand"/> by selecting the importer registered for the requested
/// <see cref="Domain.Enums.ImportSource"/> and delegating the parse to it.
/// </summary>
public sealed class ImportAccountCommandHandler : IRequestHandler<ImportAccountCommand, Account>
{
    private readonly IReadOnlyDictionary<Domain.Enums.ImportSource, IAccountImporter> _importers;

    /// <summary>Initializes the handler with every registered importer.</summary>
    /// <param name="importers">All importers available via dependency injection.</param>
    public ImportAccountCommandHandler(IEnumerable<IAccountImporter> importers)
    {
        ArgumentNullException.ThrowIfNull(importers);

        // Last registration wins for a given source, allowing a host to override the default importer.
        _importers = importers.ToDictionary(importer => importer.Source);
    }

    /// <summary>Executes the import for the requested source.</summary>
    /// <param name="request">The import command.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The imported account.</returns>
    /// <exception cref="AccountImportException">Thrown when no importer is registered for the requested source.</exception>
    public Task<Account> Handle(ImportAccountCommand request, CancellationToken cancellationToken)
    {
        if (!_importers.TryGetValue(request.Source, out IAccountImporter? importer))
        {
            throw new AccountImportException($"No importer is registered for source '{request.Source}'.");
        }

        return importer.ImportAsync(request.Content, cancellationToken);
    }
}
