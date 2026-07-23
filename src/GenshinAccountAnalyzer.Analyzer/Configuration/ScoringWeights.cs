namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Weights and targets for the weapon, artifact, build and efficiency scores. Each weight group is
/// documented to sum to 1.0 so the resulting scores stay on a 0-100 scale.
/// </summary>
public static class ScoringWeights
{
    /// <summary>Weapon rating weights (sum = 1.0): how leveled, how rare, how refined.</summary>
    public static class Weapon
    {
        /// <summary>Weight of the weapon's level completeness.</summary>
        public const double Level = 0.5d;

        /// <summary>Weight of the weapon's rarity.</summary>
        public const double Rarity = 0.3d;

        /// <summary>Weight of the weapon's refinement.</summary>
        public const double Refinement = 0.2d;
    }

    /// <summary>Artifact rating weights (sum = 1.0) and targets.</summary>
    public static class Artifact
    {
        /// <summary>Weight of the average artifact level completeness.</summary>
        public const double Level = 0.35d;

        /// <summary>Weight of the average artifact rarity.</summary>
        public const double Rarity = 0.15d;

        /// <summary>Weight of accumulated crit value from substats.</summary>
        public const double CritValue = 0.5d;

        /// <summary>Substat crit value at which the crit component is considered maxed.</summary>
        public const double TargetCritValue = 180d;
    }

    /// <summary>Build rating weights (sum = 1.0) combining the per-aspect scores.</summary>
    public static class Build
    {
        /// <summary>Weight of talent investment.</summary>
        public const double Talents = 0.30d;

        /// <summary>Weight of weapon quality.</summary>
        public const double Weapon = 0.25d;

        /// <summary>Weight of artifact quality.</summary>
        public const double Artifacts = 0.35d;

        /// <summary>Weight of character level.</summary>
        public const double Level = 0.10d;
    }

    /// <summary>Efficiency weights (sum = 1.0): how close each aspect is to "maxed".</summary>
    public static class Efficiency
    {
        /// <summary>Weight of character level completeness.</summary>
        public const double Level = 0.25d;

        /// <summary>Weight of talent completeness.</summary>
        public const double Talents = 0.25d;

        /// <summary>Weight of weapon level completeness.</summary>
        public const double Weapon = 0.25d;

        /// <summary>Weight of artifact completeness (level and slot count).</summary>
        public const double Artifacts = 0.25d;
    }
}
