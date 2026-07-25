namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>
/// A single transformative reaction proc within a <see cref="Rotation"/>. Transformative reactions
/// never crit, so a proc contributes the same value to every damage bound.
/// </summary>
public sealed record TransformativeHit
{
    /// <summary>The transformative reaction.</summary>
    public required TransformativeReaction Reaction { get; init; }

    /// <summary>The reacting character's level.</summary>
    public required int CharacterLevel { get; init; }

    /// <summary>The reacting character's Elemental Mastery.</summary>
    public required double ElementalMastery { get; init; }

    /// <summary>Extra reaction bonus (fraction).</summary>
    public double ReactionBonus { get; init; }

    /// <summary>The target's defensive profile.</summary>
    public required EnemyProfile Enemy { get; init; }
}

/// <summary>
/// A single named step in a <see cref="Rotation"/>: either a direct hit (<see cref="Hit"/>) or a
/// transformative reaction proc (<see cref="Transformative"/>) — exactly one must be set.
/// </summary>
public sealed record RotationStep
{
    /// <summary>A human-readable label (e.g. "Normal Attack 1", "Elemental Skill", "Hyperbloom proc").</summary>
    public required string Name { get; init; }

    /// <summary>The hit to evaluate, when this step is a direct attack.</summary>
    public DamageInput? Hit { get; init; }

    /// <summary>The transformative reaction to evaluate, when this step is a reaction proc.</summary>
    public TransformativeHit? Transformative { get; init; }
}

/// <summary>
/// An ordered sequence of attacks and reaction procs representing a combat rotation (e.g. one full
/// skill/burst/normal-attack combo), used to total damage across more than a single hit.
/// </summary>
public sealed record Rotation
{
    /// <summary>A human-readable label for the rotation.</summary>
    public required string Name { get; init; }

    /// <summary>The steps that make up the rotation, in execution order.</summary>
    public required IReadOnlyList<RotationStep> Steps { get; init; }

    /// <summary>
    /// The rotation's real-time duration in seconds, when known. Enables a DPS figure in the result;
    /// omit (or leave 0) when only the total damage matters.
    /// </summary>
    public double DurationSeconds { get; init; }
}
