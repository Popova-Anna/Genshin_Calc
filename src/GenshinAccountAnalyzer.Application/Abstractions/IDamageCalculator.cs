using GenshinAccountAnalyzer.Domain.Damage;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Evaluates Genshin Impact damage formulas for a single fully-resolved hit and for transformative
/// reactions. Pure and stateless: all buffs are folded into the inputs by the caller.
/// </summary>
public interface IDamageCalculator
{
    /// <summary>Computes the non-crit, crit and averaged damage of a hit.</summary>
    /// <param name="input">The resolved hit parameters.</param>
    /// <returns>The damage result.</returns>
    DamageResult CalculateHit(DamageInput input);

    /// <summary>Computes the damage of a transformative reaction.</summary>
    /// <param name="reaction">The transformative reaction.</param>
    /// <param name="characterLevel">The reacting character's level.</param>
    /// <param name="elementalMastery">The reacting character's Elemental Mastery.</param>
    /// <param name="reactionBonus">Extra reaction bonus (fraction).</param>
    /// <param name="enemy">The target's defensive profile.</param>
    /// <returns>The transformative reaction damage.</returns>
    double CalculateTransformative(
        TransformativeReaction reaction,
        int characterLevel,
        double elementalMastery,
        double reactionBonus,
        EnemyProfile enemy);

    /// <summary>
    /// Evaluates an entire rotation (sequence of hits and/or transformative procs), returning per-step
    /// results and totals — including the rotation's maximum (all-crit) and floor (no-crit) damage, and
    /// its DPS when a duration was supplied.
    /// </summary>
    /// <param name="rotation">The rotation to evaluate.</param>
    /// <returns>The rotation result.</returns>
    RotationResult CalculateRotation(Rotation rotation);
}
