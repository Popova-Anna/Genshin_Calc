namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// A single weapon candidate in a ranking: its stat-based score and how it compares to the best option.
/// </summary>
/// <param name="Id">The in-game weapon identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="Rarity">Rarity (star rating).</param>
/// <param name="Score">The stat-based score (a DPS proxy pending the damage calculator).</param>
/// <param name="RelativeToBis">Score as a percentage of the best-in-slot option's score (BiS = 100).</param>
public readonly record struct WeaponOption(int Id, string Name, int Rarity, double Score, double RelativeToBis);
