using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Turns a character's quantitative analysis into qualitative insights: strengths, weaknesses,
/// prioritized recommendations and best-in-slot gear guidance.
/// </summary>
public interface ICharacterInsightAnalyzer
{
    /// <summary>Derives insights from a character and its computed analysis.</summary>
    /// <param name="character">The character (for element, gear and identity).</param>
    /// <param name="metrics">The already-computed ratings and balance metrics.</param>
    /// <returns>The derived insights.</returns>
    CharacterInsights Analyze(Character character, CharacterAnalysis metrics);
}
