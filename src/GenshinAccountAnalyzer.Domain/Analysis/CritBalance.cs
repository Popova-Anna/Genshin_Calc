namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// Describes how a character's CRIT Rate and CRIT DMG relate to the widely used 1:2 ratio target.
/// Rate/Damage are expressed as percentages (e.g. <c>37.66</c>), matching how the game displays them.
/// </summary>
/// <param name="CritRate">Total CRIT Rate, as a percentage.</param>
/// <param name="CritDamage">Total CRIT DMG, as a percentage.</param>
/// <param name="CritValue">Crit Value of the whole sheet (<c>2 * CritRate + CritDamage</c>).</param>
/// <param name="Ratio">CRIT DMG to CRIT Rate ratio (<c>CritDamage / CritRate</c>); the target is 2.0.</param>
/// <param name="BalanceScore">How close the ratio is to the ideal, 0-100.</param>
/// <param name="IsBalanced">Whether the ratio sits within the accepted tolerance band.</param>
public readonly record struct CritBalance(
    double CritRate,
    double CritDamage,
    double CritValue,
    double Ratio,
    double BalanceScore,
    bool IsBalanced);
