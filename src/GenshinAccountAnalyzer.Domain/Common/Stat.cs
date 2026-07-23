using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Common;

/// <summary>
/// A single statistic value: a <see cref="StatType"/> paired with its magnitude.
/// Percentage stats (crit, ER, elemental bonuses, ...) are stored as fractions where <c>1.0 == 100%</c>.
/// </summary>
/// <param name="Type">The kind of statistic.</param>
/// <param name="Value">The magnitude. Fraction for percentage stats, absolute value for flat stats.</param>
public readonly record struct Stat(StatType Type, double Value);
