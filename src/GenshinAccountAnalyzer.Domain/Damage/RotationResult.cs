namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>The evaluated damage of a single <see cref="RotationStep"/>.</summary>
/// <param name="Name">The step's label, copied from the input step.</param>
/// <param name="Damage">The step's non-crit/crit/average damage.</param>
public readonly record struct RotationStepResult(string Name, DamageResult Damage);

/// <summary>
/// The evaluated result of a <see cref="Rotation"/>: per-step damage plus totals across the whole
/// sequence. <see cref="TotalCritical"/> is the rotation's maximum possible value (every hit crits);
/// <see cref="TotalNonCritical"/> is its floor (no hit crits); <see cref="TotalAverage"/> is the
/// CRIT-Rate-weighted expected total.
/// </summary>
public sealed record RotationResult
{
    /// <summary>Per-step results, in the same order as the input rotation.</summary>
    public required IReadOnlyList<RotationStepResult> Steps { get; init; }

    /// <summary>Total damage assuming no hit crits (the rotation's floor).</summary>
    public required double TotalNonCritical { get; init; }

    /// <summary>Total damage assuming every hit crits (the rotation's maximum/ceiling).</summary>
    public required double TotalCritical { get; init; }

    /// <summary>Total expected damage, weighted by each hit's CRIT Rate.</summary>
    public required double TotalAverage { get; init; }

    /// <summary>
    /// Expected damage per second (<see cref="TotalAverage"/> divided by the rotation's duration), or
    /// <c>0</c> when the rotation did not specify a duration.
    /// </summary>
    public required double Dps { get; init; }
}
