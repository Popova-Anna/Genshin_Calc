using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Produces an optimal build target for a character — recommended artifact main stats per slot and the
/// best-in-slot weapon — and measures how close the current gear is to it.
/// </summary>
public interface IBuildOptimizer
{
    /// <summary>Optimises a character's build against its computed analysis.</summary>
    /// <param name="character">The character (for equipped gear).</param>
    /// <param name="analysis">The computed analysis (for recommended main stats and best weapon).</param>
    /// <returns>The build optimisation.</returns>
    BuildOptimization Optimize(Character character, CharacterAnalysis analysis);
}
