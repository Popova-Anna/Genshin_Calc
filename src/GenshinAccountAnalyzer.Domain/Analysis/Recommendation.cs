namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// A single, actionable improvement suggestion for a character, with a priority for ordering.
/// </summary>
/// <param name="Category">A short category slug (e.g. "weapon", "talents", "artifacts", "level").</param>
/// <param name="Title">A concise summary of the action.</param>
/// <param name="Detail">A fuller explanation, including concrete values where helpful.</param>
/// <param name="Priority">How impactful the improvement is.</param>
public sealed record Recommendation(
    string Category,
    string Title,
    string Detail,
    RecommendationPriority Priority);
