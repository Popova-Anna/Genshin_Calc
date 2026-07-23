namespace GenshinAccountAnalyzer.Domain.Enums;

/// <summary>
/// Identifies the external data source an <see cref="Models.Account"/> was imported from.
/// New sources can be added here without touching existing import logic.
/// </summary>
public enum ImportSource
{
    /// <summary>Source is unknown or not recorded.</summary>
    Unknown = 0,

    /// <summary>Enka.Network showcase export.</summary>
    Enka,

    /// <summary>HoYoLab battle-chronicle export.</summary>
    HoYoLab,

    /// <summary>Akasha.cv export.</summary>
    Akasha
}
