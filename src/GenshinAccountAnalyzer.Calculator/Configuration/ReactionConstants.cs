using GenshinAccountAnalyzer.Domain.Damage;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Calculator.Configuration;

/// <summary>
/// Reaction coefficients: amplifying/additive/transformative base multipliers and their Elemental
/// Mastery bonus terms. Fixed game data, centralised to keep the calculator free of magic numbers.
/// </summary>
public static class ReactionConstants
{
    // Amplifying (Vaporize/Melt): EM bonus = coefficient * EM / (EM + base).
    /// <summary>EM numerator coefficient for amplifying reactions.</summary>
    public const double AmplifyingEmCoefficient = 2.78d;

    /// <summary>EM denominator base for amplifying reactions.</summary>
    public const double AmplifyingEmBase = 1400d;

    // Additive (Aggravate/Spread): EM bonus = coefficient * EM / (EM + base).
    /// <summary>EM numerator coefficient for additive reactions.</summary>
    public const double AdditiveEmCoefficient = 5d;

    /// <summary>EM denominator base for additive reactions.</summary>
    public const double AdditiveEmBase = 1200d;

    // Transformative: EM bonus = coefficient * EM / (EM + base).
    /// <summary>EM numerator coefficient for transformative reactions.</summary>
    public const double TransformativeEmCoefficient = 16d;

    /// <summary>EM denominator base for transformative reactions.</summary>
    public const double TransformativeEmBase = 2000d;

    private const double VaporizeForward = 1.5d;  // Pyro trigger
    private const double VaporizeReverse = 2.0d;  // Hydro trigger
    private const double MeltForward = 2.0d;      // Pyro trigger
    private const double MeltReverse = 1.5d;      // Cryo trigger

    private static readonly IReadOnlyDictionary<TransformativeReaction, double> TransformativeBaseMultipliers =
        new Dictionary<TransformativeReaction, double>
        {
            [TransformativeReaction.Overloaded] = 2.0d,
            [TransformativeReaction.Superconduct] = 0.5d,
            [TransformativeReaction.ElectroCharged] = 1.2d,
            [TransformativeReaction.Swirl] = 0.6d,
            [TransformativeReaction.Shatter] = 1.5d,
            [TransformativeReaction.Burning] = 0.25d,
            [TransformativeReaction.Bloom] = 2.0d,
            [TransformativeReaction.Hyperbloom] = 3.0d,
            [TransformativeReaction.Burgeon] = 3.0d,
        };

    /// <summary>Aggravate base multiplier.</summary>
    public const double AggravateBase = 1.15d;

    /// <summary>Spread base multiplier.</summary>
    public const double SpreadBase = 1.25d;

    /// <summary>Gets the amplifying base multiplier for a reaction and its trigger element.</summary>
    /// <param name="reaction">The amplifying reaction.</param>
    /// <param name="trigger">The element triggering the reaction.</param>
    /// <returns>The 1.5×/2× base, or <c>0</c> when there is no valid amplifying reaction.</returns>
    public static double AmplifyingBase(AmplifyingReaction reaction, ElementType trigger) => reaction switch
    {
        AmplifyingReaction.Vaporize when trigger == ElementType.Pyro => VaporizeForward,
        AmplifyingReaction.Vaporize when trigger == ElementType.Hydro => VaporizeReverse,
        AmplifyingReaction.Melt when trigger == ElementType.Pyro => MeltForward,
        AmplifyingReaction.Melt when trigger == ElementType.Cryo => MeltReverse,
        _ => 0d,
    };

    /// <summary>Gets the additive reaction base multiplier.</summary>
    /// <param name="reaction">The additive reaction.</param>
    public static double AdditiveBase(AdditiveReaction reaction) => reaction switch
    {
        AdditiveReaction.Aggravate => AggravateBase,
        AdditiveReaction.Spread => SpreadBase,
        _ => 0d,
    };

    /// <summary>Gets the transformative reaction base multiplier.</summary>
    /// <param name="reaction">The transformative reaction.</param>
    public static double TransformativeBase(TransformativeReaction reaction) =>
        TransformativeBaseMultipliers.TryGetValue(reaction, out double value) ? value : 0d;
}
