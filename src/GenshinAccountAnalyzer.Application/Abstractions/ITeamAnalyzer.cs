using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Automatically finds and ranks the best teams that can be formed from a set of characters, weighing
/// elemental resonance, reactions, roles, aggregate build strength and coherence.
/// </summary>
public interface ITeamAnalyzer
{
    /// <summary>Finds the highest-synergy teams among <paramref name="characters"/>.</summary>
    /// <param name="characters">The available characters.</param>
    /// <param name="buildScores">Overall build score by character id, used to weigh team power and roles.</param>
    /// <param name="count">Maximum number of teams to return.</param>
    /// <returns>The best teams, highest synergy first (empty when fewer than a full team is available).</returns>
    IReadOnlyList<TeamAnalysis> FindBestTeams(
        IReadOnlyList<Character> characters,
        IReadOnlyDictionary<int, double> buildScores,
        int count);
}
