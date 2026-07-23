namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// A normalized rating: a score on a 0-100 scale together with its coarse <see cref="RatingTier"/>.
/// </summary>
/// <param name="Score">The score, 0-100.</param>
/// <param name="Tier">The letter grade the score falls into.</param>
public readonly record struct Rating(double Score, RatingTier Tier);
