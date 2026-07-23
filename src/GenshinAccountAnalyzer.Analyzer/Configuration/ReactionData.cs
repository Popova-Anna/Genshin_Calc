using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Elemental reaction cores and their offensive value, plus enabler handling for Anemo (Swirl) and Geo
/// (Crystallize). Used to identify and score the dominant reaction of a team.
/// </summary>
public static class ReactionData
{
    /// <summary>Score for a cohesive mono-element team.</summary>
    public const double MonoElementScore = 0.8d;

    /// <summary>Score for an Anemo enabler swirling at least one applicable element.</summary>
    public const double SwirlScore = 0.8d;

    /// <summary>Score for a Geo crystallize/support presence.</summary>
    public const double CrystallizeScore = 0.5d;

    /// <summary>Fallback score for a team with no recognised reaction core.</summary>
    public const double MixedScore = 0.3d;

    private static readonly IReadOnlyDictionary<(ElementType, ElementType), (string Name, double Score)> Reactions =
        new Dictionary<(ElementType, ElementType), (string, double)>
        {
            [Key(ElementType.Pyro, ElementType.Hydro)] = ("Vaporize", 1.0d),
            [Key(ElementType.Pyro, ElementType.Cryo)] = ("Melt", 1.0d),
            [Key(ElementType.Electro, ElementType.Dendro)] = ("Aggravate / Quicken", 1.0d),
            [Key(ElementType.Hydro, ElementType.Dendro)] = ("Bloom", 0.9d),
            [Key(ElementType.Hydro, ElementType.Cryo)] = ("Freeze", 0.85d),
            [Key(ElementType.Pyro, ElementType.Electro)] = ("Overload", 0.7d),
            [Key(ElementType.Cryo, ElementType.Electro)] = ("Superconduct", 0.6d),
            [Key(ElementType.Hydro, ElementType.Electro)] = ("Electro-Charged", 0.6d),
            [Key(ElementType.Pyro, ElementType.Dendro)] = ("Burning", 0.5d),
        };

    private static readonly HashSet<ElementType> Swirlable =
        [ElementType.Pyro, ElementType.Hydro, ElementType.Cryo, ElementType.Electro];

    private static (ElementType, ElementType) Key(ElementType a, ElementType b) =>
        (int)a < (int)b ? (a, b) : (b, a);

    /// <summary>
    /// Identifies the dominant reaction core of a set of distinct elements and its offensive value.
    /// </summary>
    /// <param name="elements">The distinct elements present in the team.</param>
    /// <returns>The core name and a 0-1 score.</returns>
    public static (string Name, double Score) DetectCore(IReadOnlyCollection<ElementType> elements)
    {
        if (elements.Count == 1)
        {
            ElementType only = elements.First();
            return ($"Mono {only}", MonoElementScore);
        }

        string bestName = "Mixed";
        double bestScore = 0d;

        foreach ((ElementType, ElementType) pair in Reactions.Keys)
        {
            if (elements.Contains(pair.Item1) && elements.Contains(pair.Item2)
                && Reactions[pair].Score > bestScore)
            {
                (bestName, bestScore) = Reactions[pair];
            }
        }

        if (elements.Contains(ElementType.Anemo) && elements.Any(Swirlable.Contains) && SwirlScore > bestScore)
        {
            (bestName, bestScore) = ("Swirl", SwirlScore);
        }

        if (elements.Contains(ElementType.Geo) && elements.Count > 1 && CrystallizeScore > bestScore)
        {
            (bestName, bestScore) = ("Crystallize", CrystallizeScore);
        }

        return bestScore > 0d ? (bestName, bestScore) : ("Mixed", MixedScore);
    }
}
