namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>The non-crit, crit and crit-averaged damage of a single hit.</summary>
/// <param name="NonCritical">Damage assuming no critical hit.</param>
/// <param name="Critical">Damage assuming a critical hit.</param>
/// <param name="Average">Expected damage weighted by CRIT Rate.</param>
public readonly record struct DamageResult(double NonCritical, double Critical, double Average);
