using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// The result of ranking weapons for a character: the equipped weapon, the best-in-slot options and the
/// estimated performance lost by not running BiS.
/// </summary>
/// <remarks>
/// Scores are a transparent stat-based proxy (base ATK plus weighted secondary stat); exact,
/// passive-aware DPS ranking arrives with the damage calculator (Stage 9).
/// </remarks>
public sealed record WeaponAnalysis
{
    /// <summary>The in-game character (avatar) identifier.</summary>
    public required int CharacterId { get; init; }

    /// <summary>The weapon type ranked.</summary>
    public required WeaponType WeaponType { get; init; }

    /// <summary>The currently equipped weapon, when one is present.</summary>
    public WeaponOption? Equipped { get; init; }

    /// <summary>The top-ranked weapon options, best-in-slot first.</summary>
    public required IReadOnlyList<WeaponOption> Recommendations { get; init; }

    /// <summary>Estimated performance lost by the equipped weapon relative to best-in-slot, as a percentage.</summary>
    public required double DpsLossVsBis { get; init; }

    /// <summary>The name of the weight profile used to score secondary stats.</summary>
    public required string ProfileName { get; init; }
}
