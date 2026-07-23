using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>Whether a modifier adds a flat amount or a percentage.</summary>
public enum ModifierKind
{
    /// <summary>A flat, absolute amount (e.g. +50 ATK, +200 EM).</summary>
    Flat = 0,

    /// <summary>A percentage bonus as a fraction (e.g. 0.20 for +20%).</summary>
    Percent
}

/// <summary>A single stat contribution from a buff.</summary>
/// <param name="Stat">The stat affected.</param>
/// <param name="Value">The amount (absolute for <see cref="ModifierKind.Flat"/>, fraction for percent).</param>
/// <param name="Kind">Whether the value is flat or a percentage.</param>
public readonly record struct StatModifier(StatType Stat, double Value, ModifierKind Kind);

/// <summary>
/// A named collection of stat modifiers (a character/weapon/artifact passive, food, or team buff).
/// Buffs stack additively within each stat, matching the game's additive stacking of most bonuses.
/// </summary>
/// <param name="Source">A human-readable description of where the buff comes from.</param>
/// <param name="Modifiers">The stat contributions.</param>
public sealed record Buff(string Source, IReadOnlyList<StatModifier> Modifiers);
