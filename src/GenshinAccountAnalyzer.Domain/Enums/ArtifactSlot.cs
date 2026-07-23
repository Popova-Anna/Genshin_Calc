namespace GenshinAccountAnalyzer.Domain.Enums;

/// <summary>
/// The five artifact slots. Each character can equip at most one artifact per slot.
/// </summary>
public enum ArtifactSlot
{
    /// <summary>Slot could not be resolved from the imported data.</summary>
    Unknown = 0,

    /// <summary>Flower of Life (always flat HP main stat).</summary>
    Flower,

    /// <summary>Plume of Death (always flat ATK main stat).</summary>
    Plume,

    /// <summary>Sands of Eon.</summary>
    Sands,

    /// <summary>Goblet of Eonothem.</summary>
    Goblet,

    /// <summary>Circlet of Logos.</summary>
    Circlet
}
