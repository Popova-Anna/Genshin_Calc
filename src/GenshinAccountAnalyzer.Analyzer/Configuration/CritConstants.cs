namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Constants governing CRIT balance analysis.
/// </summary>
public static class CritConstants
{
    /// <summary>Weight of CRIT Rate in the Crit Value formula (<c>CV = 2 * CritRate% + CritDamage%</c>).</summary>
    public const double CritValueRateWeight = 2d;

    /// <summary>The target CRIT DMG : CRIT Rate ratio.</summary>
    public const double IdealRatio = 2d;

    /// <summary>Half-width of the ratio band still considered balanced (ideal ± tolerance).</summary>
    public const double RatioTolerance = 0.5d;

    /// <summary>Ratio distance from the ideal at which the balance score reaches zero.</summary>
    public const double MaxRatioDeviation = 2d;

    /// <summary>CRIT Rate below which the ratio is unreliable and balance cannot be assessed (percent).</summary>
    public const double MinAssessableCritRate = 5d;
}
