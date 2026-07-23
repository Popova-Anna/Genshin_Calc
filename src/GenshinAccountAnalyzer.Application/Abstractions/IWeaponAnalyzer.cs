using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Ranks weapons for a character and reports the equipped weapon's performance relative to best-in-slot.
/// </summary>
public interface IWeaponAnalyzer
{
    /// <summary>Ranks weapons for <paramref name="character"/> using the given weight profile.</summary>
    /// <param name="character">The character whose weapon options are ranked.</param>
    /// <param name="profile">The usefulness profile; when <see langword="null"/>, a generic default is used.</param>
    /// <returns>The weapon analysis.</returns>
    WeaponAnalysis Analyze(Character character, SubstatWeightProfile? profile = null);
}
