using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Calculator.Configuration;
using GenshinAccountAnalyzer.Domain.Damage;

namespace GenshinAccountAnalyzer.Calculator;

/// <summary>
/// Default <see cref="IDamageCalculator"/> implementing the core Genshin Impact damage formulas:
/// stat scaling, damage bonus, amplifying and additive reactions, crit, defense and resistance, plus
/// transformative reactions. All coefficients live in <see cref="Configuration"/> classes.
/// </summary>
public sealed class DamageCalculator : IDamageCalculator
{
    /// <inheritdoc />
    public DamageResult CalculateHit(DamageInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        double baseDamage = input.TalentMultiplier * input.ScalingStatValue
            + input.FlatDamageBonus
            + AdditiveTerm(input);

        double general = baseDamage
            * (1d + input.DamageBonus)
            * AmplifyingMultiplier(input)
            * DamageFormula.DefenseMultiplier(
                input.CharacterLevel, input.Enemy.Level, input.Enemy.DefenseReduction, input.Enemy.DefenseIgnore)
            * DamageFormula.ResistanceMultiplier(input.Enemy.Resistance - input.Enemy.ResistanceReduction);

        double critMultiplier = 1d + input.CritDamage;
        double averageMultiplier = 1d + Math.Min(1d, input.CritRate) * input.CritDamage;

        return new DamageResult(general, general * critMultiplier, general * averageMultiplier);
    }

    /// <inheritdoc />
    public double CalculateTransformative(
        TransformativeReaction reaction,
        int characterLevel,
        double elementalMastery,
        double reactionBonus,
        EnemyProfile enemy)
    {
        ArgumentNullException.ThrowIfNull(enemy);

        double emBonus = ReactionConstants.TransformativeEmCoefficient * elementalMastery
            / (elementalMastery + ReactionConstants.TransformativeEmBase);

        return ReactionConstants.TransformativeBase(reaction)
            * (1d + emBonus + reactionBonus)
            * ReactionLevelTable.ForLevel(characterLevel)
            * DamageFormula.ResistanceMultiplier(enemy.Resistance - enemy.ResistanceReduction);
    }

    private static double AmplifyingMultiplier(DamageInput input)
    {
        double baseMultiplier = ReactionConstants.AmplifyingBase(input.Amplifying, input.TriggerElement);
        if (baseMultiplier <= 0d)
        {
            return 1d;
        }

        double emBonus = ReactionConstants.AmplifyingEmCoefficient * input.ElementalMastery
            / (input.ElementalMastery + ReactionConstants.AmplifyingEmBase);

        return baseMultiplier * (1d + emBonus + input.ReactionBonus);
    }

    private static double AdditiveTerm(DamageInput input)
    {
        double baseMultiplier = ReactionConstants.AdditiveBase(input.Additive);
        if (baseMultiplier <= 0d)
        {
            return 0d;
        }

        double emBonus = ReactionConstants.AdditiveEmCoefficient * input.ElementalMastery
            / (input.ElementalMastery + ReactionConstants.AdditiveEmBase);

        return baseMultiplier
            * (1d + emBonus + input.ReactionBonus)
            * ReactionLevelTable.ForLevel(input.CharacterLevel);
    }
}
