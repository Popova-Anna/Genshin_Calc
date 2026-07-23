namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Thresholds that turn the quantitative ratings and balance metrics into qualitative strengths,
/// weaknesses and recommendations. Centralised so tuning the analyzer is a data change here.
/// </summary>
public static class InsightThresholds
{
    /// <summary>Build score at or above which the build counts as a strength.</summary>
    public const double StrongBuildScore = 80d;

    /// <summary>Efficiency at or above which the build counts as fully invested.</summary>
    public const double HighEfficiency = 90d;

    /// <summary>Efficiency below which the build is flagged as under-invested.</summary>
    public const double LowEfficiency = 50d;

    /// <summary>Weapon DPS loss (percent) at or below which the weapon counts as near-BiS.</summary>
    public const double NearBisDpsLoss = 5d;

    /// <summary>Weapon DPS loss (percent) at or above which the weapon is flagged as weak.</summary>
    public const double HighDpsLoss = 20d;

    /// <summary>Energy Recharge (fraction) below which ER is flagged as low.</summary>
    public const double MinEnergyRecharge = 1.0d;

    /// <summary>Crit value at or above which the character is considered to invest in crit.</summary>
    public const double CritInvestmentFloor = 100d;

    /// <summary>Total dead-roll value (high-roll equivalents) at or above which artifacts are flagged.</summary>
    public const double DeadRollWarnThreshold = 2.0d;

    /// <summary>Average artifact roll efficiency (percent) below which artifact quality is flagged.</summary>
    public const double LowArtifactEfficiency = 80d;

    /// <summary>Average artifact roll efficiency (percent) at or above which artifacts count as a strength.</summary>
    public const double HighArtifactEfficiency = 88d;

    /// <summary>Talent level at or above which all main talents count as maxed (strength).</summary>
    public const int MaxedTalentLevel = 9;
}
