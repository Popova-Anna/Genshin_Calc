using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// A source-specific importer that turns a raw account export stream into the analyzer's
/// internal <see cref="Account"/> model. Implement this once per data source
/// (Enka, HoYoLab, Akasha, ...) to add support without changing the rest of the system.
/// </summary>
public interface IAccountImporter
{
    /// <summary>The data source this importer understands.</summary>
    ImportSource Source { get; }

    /// <summary>
    /// Reads and parses <paramref name="stream"/> into an internal <see cref="Account"/>.
    /// </summary>
    /// <param name="stream">The raw export content. The importer reads but does not dispose it.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The imported account in the analyzer's internal model.</returns>
    /// <exception cref="Exceptions.AccountImportException">Thrown when the content cannot be parsed.</exception>
    Task<Account> ImportAsync(Stream stream, CancellationToken cancellationToken);
}
