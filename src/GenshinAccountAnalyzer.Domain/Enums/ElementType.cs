namespace GenshinAccountAnalyzer.Domain.Enums;

/// <summary>
/// Elemental (or physical) type used both for a character's vision and for damage-bonus stats.
/// </summary>
public enum ElementType
{
    /// <summary>Element could not be resolved from the imported data.</summary>
    Unknown = 0,

    /// <summary>Anemo (Wind).</summary>
    Anemo,

    /// <summary>Geo (Rock).</summary>
    Geo,

    /// <summary>Electro.</summary>
    Electro,

    /// <summary>Dendro (Grass).</summary>
    Dendro,

    /// <summary>Hydro (Water).</summary>
    Hydro,

    /// <summary>Pyro (Fire).</summary>
    Pyro,

    /// <summary>Cryo (Ice).</summary>
    Cryo,

    /// <summary>Physical (non-elemental) damage.</summary>
    Physical
}
