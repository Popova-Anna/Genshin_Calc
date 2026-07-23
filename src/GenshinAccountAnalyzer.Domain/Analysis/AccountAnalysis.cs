namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// The result of analyzing a whole account: the per-character analyses plus account-level aggregates.
/// </summary>
public sealed record AccountAnalysis
{
    /// <summary>The account UID.</summary>
    public required string Uid { get; init; }

    /// <summary>Per-character analyses.</summary>
    public required IReadOnlyList<CharacterAnalysis> Characters { get; init; }

    /// <summary>Average overall build score across all analyzed characters (0-100).</summary>
    public required double AverageBuildScore { get; init; }

    /// <summary>The best teams found on the account, highest synergy first.</summary>
    public IReadOnlyList<TeamAnalysis> Teams { get; init; } = [];
}
