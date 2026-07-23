namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// Derived qualitative insights for a character: strengths, weaknesses, prioritized recommendations and
/// best-in-slot gear suggestions. Produced from the quantitative ratings and balance metrics.
/// </summary>
public sealed record CharacterInsights
{
    /// <summary>Positive highlights of the build.</summary>
    public required IReadOnlyList<string> Strengths { get; init; }

    /// <summary>Shortcomings of the build.</summary>
    public required IReadOnlyList<string> Weaknesses { get; init; }

    /// <summary>Actionable improvements, ordered most impactful first.</summary>
    public required IReadOnlyList<Recommendation> Recommendations { get; init; }

    /// <summary>Best-in-slot weapon suggestion, when a ranking is available.</summary>
    public WeaponOption? BestWeapon { get; init; }

    /// <summary>Artifact guidance (main stats, substats, current sets).</summary>
    public required ArtifactRecommendation BestArtifacts { get; init; }
}
