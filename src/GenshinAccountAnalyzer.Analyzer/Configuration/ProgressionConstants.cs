namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Game progression caps used to normalize investment into 0-1 fractions. Centralised so a game
/// update that raises a cap is a one-line change.
/// </summary>
public static class ProgressionConstants
{
    /// <summary>Maximum character/weapon level.</summary>
    public const int MaxLevel = 90;

    /// <summary>Maximum base talent level (before constellation bonuses).</summary>
    public const int MaxTalentLevel = 10;

    /// <summary>Talent level considered "good enough" for a finished build.</summary>
    public const int TargetTalentLevel = 8;

    /// <summary>Maximum artifact enhancement level.</summary>
    public const int MaxArtifactLevel = 20;

    /// <summary>Maximum weapon refinement rank.</summary>
    public const int MaxRefinement = 5;

    /// <summary>Maximum number of constellations.</summary>
    public const int MaxConstellation = 6;

    /// <summary>Maximum rarity (star rating).</summary>
    public const int MaxRarity = 5;

    /// <summary>Number of artifact slots in a complete set.</summary>
    public const int FullArtifactSet = 5;
}
