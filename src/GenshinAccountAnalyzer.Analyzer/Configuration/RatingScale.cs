using GenshinAccountAnalyzer.Domain.Analysis;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Maps a 0-100 score to a <see cref="RatingTier"/>. Boundaries are the single source of truth for
/// how scores become letter grades across the analyzer.
/// </summary>
public static class RatingScale
{
    /// <summary>Inclusive lower bound for tier <see cref="RatingTier.SS"/>.</summary>
    public const double SsThreshold = 95d;

    /// <summary>Inclusive lower bound for tier <see cref="RatingTier.S"/>.</summary>
    public const double SThreshold = 85d;

    /// <summary>Inclusive lower bound for tier <see cref="RatingTier.A"/>.</summary>
    public const double AThreshold = 75d;

    /// <summary>Inclusive lower bound for tier <see cref="RatingTier.B"/>.</summary>
    public const double BThreshold = 60d;

    /// <summary>Inclusive lower bound for tier <see cref="RatingTier.C"/>.</summary>
    public const double CThreshold = 45d;

    /// <summary>Inclusive lower bound for tier <see cref="RatingTier.D"/>.</summary>
    public const double DThreshold = 30d;

    /// <summary>Classifies a 0-100 score into a <see cref="RatingTier"/>.</summary>
    /// <param name="score">The score to classify.</param>
    /// <returns>The matching tier.</returns>
    public static RatingTier Classify(double score) => score switch
    {
        >= SsThreshold => RatingTier.SS,
        >= SThreshold => RatingTier.S,
        >= AThreshold => RatingTier.A,
        >= BThreshold => RatingTier.B,
        >= CThreshold => RatingTier.C,
        >= DThreshold => RatingTier.D,
        _ => RatingTier.F,
    };

    /// <summary>Builds a <see cref="Rating"/> from a raw score, clamping it to 0-100 and assigning a tier.</summary>
    /// <param name="score">The raw score.</param>
    /// <returns>A clamped rating with its tier.</returns>
    public static Rating ToRating(double score)
    {
        double clamped = Math.Clamp(score, 0d, 100d);
        return new Rating(clamped, Classify(clamped));
    }
}
