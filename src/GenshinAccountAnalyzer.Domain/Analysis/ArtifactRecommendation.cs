using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// Artifact guidance for a character: recommended main stats per slot, valuable substats, and the sets
/// currently equipped. Main-stat/substat guidance is generic (element-based) until a curated build
/// dataset supplies role-specific recommendations.
/// </summary>
public sealed record ArtifactRecommendation
{
    /// <summary>Recommended main stat for each slot.</summary>
    public required IReadOnlyDictionary<ArtifactSlot, StatType> MainStats { get; init; }

    /// <summary>Valuable substats, most valuable first.</summary>
    public required IReadOnlyList<StatType> Substats { get; init; }

    /// <summary>The artifact sets currently equipped and their piece counts.</summary>
    public required IReadOnlyList<EquippedSet> CurrentSets { get; init; }

    /// <summary>A note describing how the recommendation was derived.</summary>
    public required string Notes { get; init; }
}
