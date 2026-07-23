using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Models;

/// <summary>
/// A single artifact equipped by a character, in the analyzer's own source-independent representation.
/// </summary>
public sealed record Artifact
{
    /// <summary>The in-game artifact (reliquary) identifier.</summary>
    public required int Id { get; init; }

    /// <summary>The in-game set identifier.</summary>
    public required int SetId { get; init; }

    /// <summary>Set display name, or a generated placeholder when metadata is unavailable.</summary>
    public required string SetName { get; init; }

    /// <summary>The slot this artifact occupies.</summary>
    public required ArtifactSlot Slot { get; init; }

    /// <summary>Rarity (star rating), 1-5.</summary>
    public required int Rarity { get; init; }

    /// <summary>Enhancement level, 0-20.</summary>
    public required int Level { get; init; }

    /// <summary>The main stat.</summary>
    public required Stat MainStat { get; init; }

    /// <summary>The sub (secondary) stats, up to four.</summary>
    public required IReadOnlyList<Stat> SubStats { get; init; }

    /// <summary>
    /// Total number of substat rolls this artifact has received (initial substats plus every
    /// enhancement roll). Zero when the source does not report per-roll data.
    /// </summary>
    public int RollCount { get; init; }
}
