namespace GenshinAccountAnalyzer.Domain.Common;

/// <summary>
/// A short piece of user-facing text in the languages the app supports (English and Russian).
/// Keeping both allows the presentation layer to switch language without re-running analysis.
/// </summary>
/// <param name="En">English text.</param>
/// <param name="Ru">Russian text.</param>
public readonly record struct LocalizedText(string En, string Ru)
{
    /// <summary>Creates a value where both languages share the same text.</summary>
    /// <param name="text">The text.</param>
    public static LocalizedText Same(string text) => new(text, text);
}
