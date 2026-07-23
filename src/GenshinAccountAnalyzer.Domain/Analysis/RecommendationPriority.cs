namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>How impactful acting on a recommendation is, from lowest to highest.</summary>
public enum RecommendationPriority
{
    /// <summary>Minor, optional improvement.</summary>
    Low = 0,

    /// <summary>Worthwhile improvement.</summary>
    Medium,

    /// <summary>High-impact improvement that should be addressed first.</summary>
    High
}
