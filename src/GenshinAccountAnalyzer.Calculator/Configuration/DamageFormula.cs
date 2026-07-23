namespace GenshinAccountAnalyzer.Calculator.Configuration;

/// <summary>
/// Constants and helpers for the core defense and resistance multipliers shared by every hit.
/// </summary>
public static class DamageFormula
{
    /// <summary>The constant added to attacker and defender levels in the defense formula.</summary>
    public const double LevelConstant = 100d;

    /// <summary>Resistance below which the "negative resistance" branch applies.</summary>
    public const double LowResistanceBound = 0d;

    /// <summary>Resistance at or above which the "high resistance" branch applies.</summary>
    public const double HighResistanceBound = 0.75d;

    /// <summary>Divisor applied to negative resistance (<c>1 - res/2</c>).</summary>
    public const double NegativeResistanceDivisor = 2d;

    /// <summary>Factor in the high-resistance branch (<c>1 / (4·res + 1)</c>).</summary>
    public const double HighResistanceFactor = 4d;

    /// <summary>
    /// Computes the defense multiplier: <c>(atkLvl+100) / ((atkLvl+100) + (defLvl+100)·(1-shred)·(1-ignore))</c>.
    /// </summary>
    /// <param name="attackerLevel">Attacking character level.</param>
    /// <param name="enemyLevel">Target level.</param>
    /// <param name="defenseReduction">Defense shred applied to the target (fraction).</param>
    /// <param name="defenseIgnore">Defense ignored by the attacker (fraction).</param>
    /// <returns>The defense multiplier (0-1).</returns>
    public static double DefenseMultiplier(
        int attackerLevel,
        int enemyLevel,
        double defenseReduction,
        double defenseIgnore)
    {
        double attacker = attackerLevel + LevelConstant;
        double defender = (enemyLevel + LevelConstant)
            * (1d - defenseReduction)
            * (1d - defenseIgnore);
        return attacker / (attacker + defender);
    }

    /// <summary>
    /// Computes the resistance multiplier from an effective resistance (base minus reduction).
    /// </summary>
    /// <param name="resistance">Effective resistance as a fraction.</param>
    /// <returns>The resistance multiplier.</returns>
    public static double ResistanceMultiplier(double resistance)
    {
        if (resistance < LowResistanceBound)
        {
            return 1d - resistance / NegativeResistanceDivisor;
        }

        if (resistance < HighResistanceBound)
        {
            return 1d - resistance;
        }

        return 1d / (HighResistanceFactor * resistance + 1d);
    }
}
