using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="ITeamAnalyzer"/>: enumerates every full team from the roster and scores each on
/// reaction core, elemental resonance, aggregate build strength and coherence. All coefficients live in
/// <see cref="Configuration"/> classes.
/// </summary>
public sealed class TeamAnalyzer : ITeamAnalyzer
{
    private const double PercentScale = 100d;

    /// <inheritdoc />
    public IReadOnlyList<TeamAnalysis> FindBestTeams(
        IReadOnlyList<Character> characters,
        IReadOnlyDictionary<int, double> buildScores,
        int count)
    {
        ArgumentNullException.ThrowIfNull(characters);
        ArgumentNullException.ThrowIfNull(buildScores);

        if (characters.Count < TeamScoreWeights.TeamSize || count <= 0)
        {
            return [];
        }

        return Combinations(characters.Count, TeamScoreWeights.TeamSize)
            .Select(indices => ScoreTeam(indices.Select(i => characters[i]).ToList(), buildScores))
            .OrderByDescending(team => team.Score)
            .Take(count)
            .ToList();
    }

    private static TeamAnalysis ScoreTeam(IReadOnlyList<Character> members, IReadOnlyDictionary<int, double> buildScores)
    {
        List<ElementType> elements = members.Select(m => m.Element).ToList();
        List<ElementType> distinct = elements.Distinct().ToList();

        (string coreName, double reactionScore) = ReactionData.DetectCore(distinct);
        (List<TeamResonance> resonances, double resonanceScore) = DetectResonances(elements);
        double powerScore = PowerScore(members, buildScores);
        double coherence = Coherence(distinct.Count);

        double total = (TeamScoreWeights.Reaction * reactionScore
            + TeamScoreWeights.Resonance * resonanceScore
            + TeamScoreWeights.Power * powerScore
            + TeamScoreWeights.Coherence * coherence) * PercentScale;

        List<TeamMember> orderedMembers = BuildMembers(members, buildScores);

        return new TeamAnalysis
        {
            Members = orderedMembers,
            Score = total,
            ReactionCore = coreName,
            Resonances = resonances,
            Reasons = BuildReasons(coreName, resonances, elements),
        };
    }

    private static (List<TeamResonance> Resonances, double Score) DetectResonances(IReadOnlyList<ElementType> elements)
    {
        var resonances = new List<TeamResonance>();
        double score = 0d;

        foreach (IGrouping<ElementType, ElementType> group in elements.GroupBy(e => e))
        {
            if (group.Count() < ResonanceData.MinCharactersForResonance)
            {
                continue;
            }

            if (ResonanceData.ForElement(group.Key) is { } resonance)
            {
                resonances.Add(resonance);
                score += ResonanceData.Value(group.Key);
            }
        }

        return (resonances, Math.Min(1d, score));
    }

    private static double PowerScore(IReadOnlyList<Character> members, IReadOnlyDictionary<int, double> buildScores)
    {
        List<double> scores = members
            .Select(m => buildScores.TryGetValue(m.Id, out double s) ? s : 0d)
            .OrderByDescending(s => s)
            .ToList();

        double weighted = 0d;
        for (int i = 0; i < scores.Count && i < TeamScoreWeights.MemberPowerWeights.Count; i++)
        {
            weighted += scores[i] * TeamScoreWeights.MemberPowerWeights[i];
        }

        return weighted / PercentScale;
    }

    private static double Coherence(int distinctElements)
    {
        IReadOnlyList<double> table = TeamScoreWeights.CoherenceByDistinctElements;
        return distinctElements < table.Count ? table[distinctElements] : table[^1];
    }

    private static List<TeamMember> BuildMembers(
        IReadOnlyList<Character> members,
        IReadOnlyDictionary<int, double> buildScores)
    {
        return members
            .OrderByDescending(m => buildScores.TryGetValue(m.Id, out double s) ? s : 0d)
            .Select((m, index) => new TeamMember(
                m.Id,
                m.Name,
                m.Element,
                buildScores.TryGetValue(m.Id, out double score) ? score : 0d,
                index < TeamScoreWeights.RolesByStrength.Count
                    ? TeamScoreWeights.RolesByStrength[index]
                    : "Support"))
            .ToList();
    }

    private static List<string> BuildReasons(
        string coreName,
        IReadOnlyList<TeamResonance> resonances,
        IReadOnlyList<ElementType> elements)
    {
        var reasons = new List<string> { $"Reaction core: {coreName}" };

        foreach (TeamResonance resonance in resonances)
        {
            reasons.Add($"{resonance.Name} resonance ({resonance.Effect})");
        }

        int electroCount = elements.Count(e => e == ElementType.Electro);
        reasons.Add(electroCount >= ResonanceData.MinCharactersForResonance
            ? "Electro resonance supports energy generation"
            : "Ensure sufficient Energy Recharge to keep bursts online");

        return reasons;
    }

    /// <summary>Yields every combination of <paramref name="k"/> indices from <c>[0, n)</c>.</summary>
    private static IEnumerable<int[]> Combinations(int n, int k)
    {
        int[] indices = new int[k];
        for (int i = 0; i < k; i++)
        {
            indices[i] = i;
        }

        while (true)
        {
            yield return (int[])indices.Clone();

            int position = k - 1;
            while (position >= 0 && indices[position] == n - k + position)
            {
                position--;
            }

            if (position < 0)
            {
                yield break;
            }

            indices[position]++;
            for (int i = position + 1; i < k; i++)
            {
                indices[i] = indices[i - 1] + 1;
            }
        }
    }
}
