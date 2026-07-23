namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Weights and constants for scoring and ranking teams. The four component weights sum to 1.0 so the
/// team score stays on a 0-100 scale.
/// </summary>
public static class TeamScoreWeights
{
    /// <summary>Number of characters in a team.</summary>
    public const int TeamSize = 4;

    /// <summary>Default number of top teams to return.</summary>
    public const int DefaultTeamCount = 5;

    /// <summary>Weight of the dominant reaction core.</summary>
    public const double Reaction = 0.30d;

    /// <summary>Weight of active elemental resonances.</summary>
    public const double Resonance = 0.15d;

    /// <summary>Weight of aggregate build strength.</summary>
    public const double Power = 0.45d;

    /// <summary>Weight of elemental coherence.</summary>
    public const double Coherence = 0.10d;

    /// <summary>
    /// Per-member weights for the aggregate power component (equal, so every slot's build matters and a
    /// weak member drags the team down); sum to 1.0.
    /// </summary>
    public static IReadOnlyList<double> MemberPowerWeights { get; } = [0.25d, 0.25d, 0.25d, 0.25d];

    /// <summary>Coherence value by number of distinct elements in the team (index 0 unused).</summary>
    public static IReadOnlyList<double> CoherenceByDistinctElements { get; } = [0d, 0.85d, 1.0d, 0.95d, 0.85d];

    /// <summary>Roles assigned by descending build strength within a team.</summary>
    public static IReadOnlyList<string> RolesByStrength { get; } = ["Carry", "Sub-DPS", "Support", "Support"];
}
