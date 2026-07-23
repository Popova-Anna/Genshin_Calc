using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// Assigns a usefulness weight (0-1) to each substat, describing how valuable that stat is for a
/// particular character or role. A weight of <c>0</c> marks a stat as useless (its rolls are "dead").
/// Role-specific profiles are supplied by later stages; a generic default is provided by the analyzer.
/// </summary>
public sealed class SubstatWeightProfile
{
    private readonly IReadOnlyDictionary<StatType, double> _weights;

    /// <summary>Initializes a profile.</summary>
    /// <param name="name">A human-readable profile name (e.g. "Generic Crit DPS").</param>
    /// <param name="weights">Per-stat usefulness weights in the range 0-1.</param>
    public SubstatWeightProfile(string name, IReadOnlyDictionary<StatType, double> weights)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _weights = weights ?? throw new ArgumentNullException(nameof(weights));
    }

    /// <summary>The profile's name.</summary>
    public string Name { get; }

    /// <summary>Gets the usefulness weight for <paramref name="type"/>, or <c>0</c> when unspecified.</summary>
    /// <param name="type">The substat type.</param>
    public double this[StatType type] => _weights.TryGetValue(type, out double weight) ? weight : 0d;
}
