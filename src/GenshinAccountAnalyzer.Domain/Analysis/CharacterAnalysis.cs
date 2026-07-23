namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// The result of analyzing a single character: progression, per-aspect ratings and balance metrics.
/// Later stages extend this with strengths/weaknesses, recommendations, best teams/weapons/artifacts
/// and damage/energy profiles.
/// </summary>
public sealed record CharacterAnalysis
{
    /// <summary>The in-game character (avatar) identifier.</summary>
    public required int CharacterId { get; init; }

    /// <summary>Display name.</summary>
    public required string Name { get; init; }

    /// <summary>The character's element.</summary>
    public Enums.ElementType Element { get; init; } = Enums.ElementType.Unknown;

    /// <summary>Current level.</summary>
    public required int Level { get; init; }

    /// <summary>Maximum attainable level.</summary>
    public required int MaxLevel { get; init; }

    /// <summary>Number of unlocked constellations, 0-6.</summary>
    public required int ConstellationLevel { get; init; }

    /// <summary>The three main talent levels, when they could be identified from metadata.</summary>
    public TalentLevels? Talents { get; init; }

    /// <summary>Rating of the character's talent investment.</summary>
    public required Rating TalentRating { get; init; }

    /// <summary>Rating of the equipped weapon's quality/investment.</summary>
    public required Rating WeaponRating { get; init; }

    /// <summary>Rating of the equipped artifacts.</summary>
    public required Rating ArtifactRating { get; init; }

    /// <summary>Overall build rating combining talents, weapon and artifacts.</summary>
    public required Rating BuildRating { get; init; }

    /// <summary>The overall build score (0-100), mirrored from <see cref="BuildRating"/> for convenience.</summary>
    public required double OverallScore { get; init; }

    /// <summary>CRIT Rate / CRIT DMG balance analysis.</summary>
    public required CritBalance CritBalance { get; init; }

    /// <summary>Total Energy Recharge, as a fraction (e.g. <c>1.22</c> for 122%).</summary>
    public required double EnergyRecharge { get; init; }

    /// <summary>Total Elemental Mastery.</summary>
    public required double ElementalMastery { get; init; }

    /// <summary>How "finished" the build is (level/talents/weapon/artifacts maxed), 0-100.</summary>
    public required double Efficiency { get; init; }

    /// <summary>Per-artifact analysis (crit value, roll value, efficiency, dead rolls).</summary>
    public IReadOnlyList<ArtifactAnalysis> Artifacts { get; init; } = [];

    /// <summary>Weapon ranking: best-in-slot options and the equipped weapon's loss versus BiS.</summary>
    public WeaponAnalysis? Weapon { get; init; }

    /// <summary>Positive highlights of the build.</summary>
    public IReadOnlyList<string> Strengths { get; init; } = [];

    /// <summary>Shortcomings of the build.</summary>
    public IReadOnlyList<string> Weaknesses { get; init; } = [];

    /// <summary>Actionable improvements, ordered most impactful first.</summary>
    public IReadOnlyList<Recommendation> Recommendations { get; init; } = [];

    /// <summary>Best-in-slot weapon suggestion, when a ranking is available.</summary>
    public WeaponOption? BestWeapon { get; init; }

    /// <summary>Artifact guidance (main stats, substats, current sets), when available.</summary>
    public ArtifactRecommendation? BestArtifacts { get; init; }

    /// <summary>The best teams on the account that feature this character, highest synergy first.</summary>
    public IReadOnlyList<TeamAnalysis> BestTeams { get; init; } = [];
}
