namespace GenshinAccountAnalyzer.Domain.Enums;

/// <summary>
/// The five weapon categories in Genshin Impact. A character can only equip weapons of their own type.
/// </summary>
public enum WeaponType
{
    /// <summary>Weapon type could not be resolved from the imported data.</summary>
    Unknown = 0,

    /// <summary>One-handed sword.</summary>
    Sword,

    /// <summary>Claymore (two-handed sword).</summary>
    Claymore,

    /// <summary>Polearm.</summary>
    Polearm,

    /// <summary>Bow.</summary>
    Bow,

    /// <summary>Catalyst.</summary>
    Catalyst
}
