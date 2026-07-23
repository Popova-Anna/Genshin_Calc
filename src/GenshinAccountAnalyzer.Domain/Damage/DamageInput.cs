using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>
/// A fully-resolved single hit to be evaluated by the damage calculator. All buffs (character, weapon,
/// artifact, food, team) are expected to already be folded into the stat fields — the calculator is a
/// pure function of this input, so snapshot vs dynamic buffs are the caller's concern.
/// </summary>
public sealed record DamageInput
{
    /// <summary>Level of the attacking character (used by the defense formula).</summary>
    public required int CharacterLevel { get; init; }

    /// <summary>Talent multiplier as a fraction (e.g. <c>1.5</c> for 150%).</summary>
    public required double TalentMultiplier { get; init; }

    /// <summary>Which stat the hit scales from.</summary>
    public ScalingType Scaling { get; init; } = ScalingType.Atk;

    /// <summary>The total value of the scaling stat (total ATK/HP/DEF).</summary>
    public required double ScalingStatValue { get; init; }

    /// <summary>Flat additive base damage (e.g. from certain talents/passives), before multipliers.</summary>
    public double FlatDamageBonus { get; init; }

    /// <summary>Sum of all damage-bonus percentages that apply to this hit (fraction).</summary>
    public double DamageBonus { get; init; }

    /// <summary>CRIT Rate (fraction).</summary>
    public double CritRate { get; init; }

    /// <summary>CRIT DMG (fraction).</summary>
    public double CritDamage { get; init; }

    /// <summary>Elemental Mastery, used by reaction terms.</summary>
    public double ElementalMastery { get; init; }

    /// <summary>Amplifying reaction on this hit, if any.</summary>
    public AmplifyingReaction Amplifying { get; init; } = AmplifyingReaction.None;

    /// <summary>The element triggering the amplifying reaction (determines the 1.5×/2× base).</summary>
    public ElementType TriggerElement { get; init; } = ElementType.Unknown;

    /// <summary>Additive (Aggravate/Spread) reaction on this hit, if any.</summary>
    public AdditiveReaction Additive { get; init; } = AdditiveReaction.None;

    /// <summary>Extra reaction bonus from artifacts/passives (fraction), applied to reaction terms.</summary>
    public double ReactionBonus { get; init; }

    /// <summary>The target's defensive profile.</summary>
    public required EnemyProfile Enemy { get; init; }
}
