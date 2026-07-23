using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Models;

/// <summary>
/// The root aggregate: a fully imported Genshin Impact account, independent of the source it came from.
/// </summary>
public sealed record Account
{
    /// <summary>The account UID.</summary>
    public required string Uid { get; init; }

    /// <summary>Player-level profile information.</summary>
    public required Profile Profile { get; init; }

    /// <summary>The characters showcased on the account.</summary>
    public required IReadOnlyList<Character> Characters { get; init; }

    /// <summary>The source this account was imported from.</summary>
    public ImportSource Source { get; init; } = ImportSource.Unknown;

    /// <summary>The instant (UTC) the account was imported.</summary>
    public DateTimeOffset ImportedAt { get; init; } = DateTimeOffset.UtcNow;
}
