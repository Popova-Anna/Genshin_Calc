using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Maps an element to its damage-bonus stat, used to recommend a goblet main stat from a character's vision.
/// </summary>
public static class ElementDamageBonus
{
    private static readonly IReadOnlyDictionary<ElementType, StatType> Map = new Dictionary<ElementType, StatType>
    {
        [ElementType.Anemo] = StatType.AnemoDamageBonus,
        [ElementType.Geo] = StatType.GeoDamageBonus,
        [ElementType.Electro] = StatType.ElectroDamageBonus,
        [ElementType.Dendro] = StatType.DendroDamageBonus,
        [ElementType.Hydro] = StatType.HydroDamageBonus,
        [ElementType.Pyro] = StatType.PyroDamageBonus,
        [ElementType.Cryo] = StatType.CryoDamageBonus,
        [ElementType.Physical] = StatType.PhysicalDamageBonus,
    };

    /// <summary>Gets the damage-bonus stat for an element, or <see cref="StatType.None"/> when unknown.</summary>
    /// <param name="element">The element.</param>
    public static StatType ForElement(ElementType element) =>
        Map.TryGetValue(element, out StatType value) ? value : StatType.None;
}
