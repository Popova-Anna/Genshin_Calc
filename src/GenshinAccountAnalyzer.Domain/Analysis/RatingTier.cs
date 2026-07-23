namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// A coarse letter grade for a numeric score, ordered from worst (<see cref="F"/>) to best (<see cref="SS"/>).
/// </summary>
public enum RatingTier
{
    /// <summary>Failing / barely built.</summary>
    F = 0,

    /// <summary>Poor.</summary>
    D,

    /// <summary>Below average.</summary>
    C,

    /// <summary>Average.</summary>
    B,

    /// <summary>Good.</summary>
    A,

    /// <summary>Excellent.</summary>
    S,

    /// <summary>Exceptional.</summary>
    SS
}
