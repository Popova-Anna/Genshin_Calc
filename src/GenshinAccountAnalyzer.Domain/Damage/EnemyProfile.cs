namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>
/// The defensive profile of a target used by the damage formulas. Resistance values are fractions
/// (e.g. <c>0.1</c> for 10%).
/// </summary>
public sealed record EnemyProfile
{
    /// <summary>Target level, used by the defense formula.</summary>
    public required int Level { get; init; }

    /// <summary>Base elemental/physical resistance of the relevant damage type (fraction).</summary>
    public double Resistance { get; init; }

    /// <summary>Resistance reduction applied to the target (fraction).</summary>
    public double ResistanceReduction { get; init; }

    /// <summary>Defense reduction (shred) applied to the target (fraction).</summary>
    public double DefenseReduction { get; init; }

    /// <summary>Defense ignored by the attacker (fraction).</summary>
    public double DefenseIgnore { get; init; }

    /// <summary>A level-90 target with 10% resistance and no shred, a common benchmark.</summary>
    public static EnemyProfile Standard90 { get; } = new() { Level = 90, Resistance = 0.1 };
}
