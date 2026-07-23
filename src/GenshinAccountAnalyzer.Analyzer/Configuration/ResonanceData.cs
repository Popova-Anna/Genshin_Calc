using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// Elemental resonance definitions (name, effect, offensive value) keyed by element. A resonance is
/// active when a team has two or more characters of that element.
/// </summary>
public static class ResonanceData
{
    private sealed record ResonanceDefinition(string Name, string Effect, double Value);

    private static readonly IReadOnlyDictionary<ElementType, ResonanceDefinition> Definitions =
        new Dictionary<ElementType, ResonanceDefinition>
        {
            [ElementType.Pyro] = new("Fervent Flames", "+25% ATK", 1.0d),
            [ElementType.Dendro] = new("Sprawling Greenery", "+Elemental Mastery and reaction bonuses", 0.9d),
            [ElementType.Cryo] = new("Shattering Ice", "+15% CRIT Rate against affected enemies", 0.8d),
            [ElementType.Hydro] = new("Soothing Water", "+25% max HP", 0.7d),
            [ElementType.Geo] = new("Enduring Rock", "+shield strength and shielded DMG", 0.7d),
            [ElementType.Electro] = new("High Voltage", "energy and particle generation", 0.6d),
            [ElementType.Anemo] = new("Impetuous Winds", "-stamina, +movement and cooldown", 0.3d),
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
