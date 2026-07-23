using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// An active elemental resonance in a team (two or more characters of the same element).
/// </summary>
/// <param name="Element">The resonating element.</param>
/// <param name="Name">The resonance name (e.g. "Fervent Flames").</param>
/// <param name="Effect">A short description of the resonance effect.</param>
public readonly record struct TeamResonance(ElementType Element, string Name, string Effect);
