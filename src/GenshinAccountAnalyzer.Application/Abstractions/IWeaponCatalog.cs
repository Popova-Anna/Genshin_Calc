using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// A catalog of all known weapons with their level-90 stats, used to rank weapon options for a character.
/// Backed by patch-versioned data, so new weapons are a data change, never a code change.
/// </summary>
public interface IWeaponCatalog
{
    /// <summary>Gets a weapon by id.</summary>
    /// <param name="weaponId">The in-game weapon identifier.</param>
    /// <returns>The weapon, or <see langword="null"/> when unknown.</returns>
    WeaponInfo? Get(int weaponId);

    /// <summary>Gets all catalog weapons of a given type.</summary>
    /// <param name="type">The weapon type to filter by.</param>
    /// <returns>The matching weapons (possibly empty).</returns>
    IReadOnlyList<WeaponInfo> GetByType(WeaponType type);
}

/// <summary>Full catalog information for a weapon at level 90.</summary>
/// <param name="Id">The in-game weapon identifier.</param>
/// <param name="Name">Localized display name.</param>
/// <param name="Type">The weapon category.</param>
/// <param name="Rarity">Rarity (star rating).</param>
/// <param name="BaseAttack">Base ATK at level 90.</param>
/// <param name="SecondaryStat">The secondary stat type, or <see cref="StatType.None"/>.</param>
/// <param name="SecondaryValue">The secondary stat value at level 90 (fraction for percentage stats).</param>
public sealed record WeaponInfo(
    int Id,
    string Name,
    WeaponType Type,
    int Rarity,
    double BaseAttack,
    StatType SecondaryStat,
    double SecondaryValue);
