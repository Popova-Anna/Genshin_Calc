using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Analyzes a single character, producing progression, per-aspect ratings and balance metrics.
/// </summary>
public interface ICharacterAnalyzer
{
    /// <summary>Analyzes <paramref name="character"/>.</summary>
    /// <param name="character">The character to analyze.</param>
    /// <returns>The character analysis.</returns>
    CharacterAnalysis Analyze(Character character);
}
