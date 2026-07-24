using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Elemental reaction cores and their offensive value, plus enabler handling for Anemo (Swirl) and Geo
/// (Crystallize). Used to identify and score the dominant, localized reaction of a team.
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

    private static readonly IReadOnlyDictionary<(ElementType, ElementType), (LocalizedText Name, double Score)> Reactions =
        new Dictionary<(ElementType, ElementType), (LocalizedText, double)>
        {
            [Key(ElementType.Pyro, ElementType.Hydro)] = (new("Vaporize", "Пар"), 1.0d),
            [Key(ElementType.Pyro, ElementType.Cryo)] = (new("Melt", "Таяние"), 1.0d),
            [Key(ElementType.Electro, ElementType.Dendro)] = (new("Aggravate / Quicken", "Обострение / Стимуляция"), 1.0d),
            [Key(ElementType.Hydro, ElementType.Dendro)] = (new("Bloom", "Цветение"), 0.9d),
            [Key(ElementType.Hydro, ElementType.Cryo)] = (new("Freeze", "Заморозка"), 0.85d),
            [Key(ElementType.Pyro, ElementType.Electro)] = (new("Overload", "Перегрузка"), 0.7d),
            [Key(ElementType.Cryo, ElementType.Electro)] = (new("Superconduct", "Сверхпроводник"), 0.6d),
            [Key(ElementType.Hydro, ElementType.Electro)] = (new("Electro-Charged", "Заряжен"), 0.6d),
            [Key(ElementType.Pyro, ElementType.Dendro)] = (new("Burning", "Горение"), 0.5d),
        };

    private static readonly HashSet<ElementType> Swirlable =
        [ElementType.Pyro, ElementType.Hydro, ElementType.Cryo, ElementType.Electro];

    private static (ElementType, ElementType) Key(ElementType a, ElementType b) =>
        (int)a < (int)b ? (a, b) : (b, a);

    /// <summary>
    /// Identifies the dominant, localized reaction core of a set of distinct elements and its value.
    /// </summary>
    /// <param name="elements">The distinct elements present in the team.</param>
    /// <returns>The localized core name and a 0-1 score.</returns>
    public static (LocalizedText Name, double Score) DetectCore(IReadOnlyCollection<ElementType> elements)
    {
        if (elements.Count == 1)
        {
            ElementType only = elements.First();
            return (new LocalizedText($"Mono {only}", $"Моно {only}"), MonoElementScore);
        }

        LocalizedText bestName = new("Mixed", "Смешанная");
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
            (bestName, bestScore) = (new LocalizedText("Swirl", "Рассеивание"), SwirlScore);
        }

        if (elements.Contains(ElementType.Geo) && elements.Count > 1 && CrystallizeScore > bestScore)
        {
            (bestName, bestScore) = (new LocalizedText("Crystallize", "Кристаллизация"), CrystallizeScore);
        }

        return bestScore > 0d ? (bestName, bestScore) : (new LocalizedText("Mixed", "Смешанная"), MixedScore);
    }
}
