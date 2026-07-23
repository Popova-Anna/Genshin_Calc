namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// A ranked candidate team: its members, overall synergy score, dominant reaction core, active
/// resonances and human-readable reasons behind the score.
/// </summary>
public sealed record TeamAnalysis
{
    /// <summary>The team members, strongest (carry) first.</summary>
    public required IReadOnlyList<TeamMember> Members { get; init; }

    /// <summary>Overall synergy score, 0-100.</summary>
    public required double Score { get; init; }

    /// <summary>The dominant reaction or cohesion core (e.g. "Vaporize", "Mono Pyro", "Aggravate").</summary>
    public required string ReactionCore { get; init; }

    /// <summary>Active elemental resonances.</summary>
    public required IReadOnlyList<TeamResonance> Resonances { get; init; }

    /// <summary>Human-readable notes explaining the team's synergy and energy considerations.</summary>
    public required IReadOnlyList<string> Reasons { get; init; }
}
