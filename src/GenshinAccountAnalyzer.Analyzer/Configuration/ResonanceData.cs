using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Elemental resonance definitions (bilingual name and effect, offensive value) keyed by element. A
/// resonance is active when a team has two or more characters of that element.
/// </summary>
public static class ResonanceData
{
    private sealed record ResonanceDefinition(LocalizedText Name, LocalizedText Effect, double Value);

    private static readonly IReadOnlyDictionary<ElementType, ResonanceDefinition> Definitions =
        new Dictionary<ElementType, ResonanceDefinition>
        {
            [ElementType.Pyro] = new(
                new("Fervent Flames", "Пылающее пламя"), new("+25% ATK", "+25% к силе атаки"), 1.0d),
            [ElementType.Dendro] = new(
                new("Sprawling Greenery", "Буйная зелень"),
                new("+Elemental Mastery and reaction bonuses", "+Мастерство стихий и бонусы реакций"), 0.9d),
            [ElementType.Cryo] = new(
                new("Shattering Ice", "Дробящий лёд"),
                new("+15% CRIT Rate against affected enemies", "+15% к шансу крит. попадания по поражённым врагам"), 0.8d),
            [ElementType.Hydro] = new(
                new("Soothing Water", "Ласковая вода"), new("+25% max HP", "+25% к макс. HP"), 0.7d),
            [ElementType.Geo] = new(
                new("Enduring Rock", "Стойкий камень"),
                new("+shield strength and shielded DMG", "+прочность щита и урон под щитом"), 0.7d),
            [ElementType.Electro] = new(
                new("High Voltage", "Высокое напряжение"),
                new("energy and particle generation", "генерация энергии и частиц"), 0.6d),
            [ElementType.Anemo] = new(
                new("Impetuous Winds", "Стремительный ветер"),
                new("-stamina, +movement and cooldown", "-выносливость, +скорость и перезарядка"), 0.3d),
        };

    /// <summary>The minimum same-element characters needed for a resonance to be active.</summary>
    public const int MinCharactersForResonance = 2;

    /// <summary>Returns the resonance for an element, when it has one.</summary>
    /// <param name="element">The element.</param>
    /// <returns>The resonance, or <see langword="null"/> when the element has no resonance.</returns>
    public static TeamResonance? ForElement(ElementType element) =>
        Definitions.TryGetValue(element, out ResonanceDefinition? definition)
            ? new TeamResonance(element, definition.Name, definition.Effect)
            : null;

    /// <summary>Returns the offensive value of an element's resonance, or <c>0</c> when it has none.</summary>
    /// <param name="element">The element.</param>
    public static double Value(ElementType element) =>
        Definitions.TryGetValue(element, out ResonanceDefinition? definition) ? definition.Value : 0d;
}
