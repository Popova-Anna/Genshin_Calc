using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Parser.Enka.Mapping;

/// <summary>
/// String-keyed mappings for Enka's resolved (<c>flat</c>) data: appendProp ids, equip types and weapon icons.
/// Percentage handling is centralised here so no conversion "magic numbers" leak into the importer.
/// </summary>
internal static class EnkaStatMaps
{
    /// <summary>Percent stat values in the <c>flat</c> block are expressed in points (e.g. 21.8 == 21.8%).</summary>
    private const double PercentToFraction = 100d;

    /// <summary>Weapon base ATK appendProp id.</summary>
    public const string WeaponBaseAttackProp = "FIGHT_PROP_BASE_ATTACK";

    /// <summary>Maps Enka <c>appendPropId</c> / <c>mainPropId</c> strings to <see cref="StatType"/>.</summary>
    public static IReadOnlyDictionary<string, StatType> AppendPropToStatType { get; } =
        new Dictionary<string, StatType>(StringComparer.Ordinal)
        {
            ["FIGHT_PROP_HP"] = StatType.Hp,
            ["FIGHT_PROP_HP_PERCENT"] = StatType.HpPercent,
            ["FIGHT_PROP_ATTACK"] = StatType.Atk,
            ["FIGHT_PROP_ATTACK_PERCENT"] = StatType.AtkPercent,
            ["FIGHT_PROP_DEFENSE"] = StatType.Def,
            ["FIGHT_PROP_DEFENSE_PERCENT"] = StatType.DefPercent,
            ["FIGHT_PROP_CRITICAL"] = StatType.CritRate,
            ["FIGHT_PROP_CRITICAL_HURT"] = StatType.CritDamage,
            ["FIGHT_PROP_CHARGE_EFFICIENCY"] = StatType.EnergyRecharge,
            ["FIGHT_PROP_HEAL_ADD"] = StatType.HealingBonus,
            ["FIGHT_PROP_ELEMENT_MASTERY"] = StatType.ElementalMastery,
            ["FIGHT_PROP_PHYSICAL_ADD_HURT"] = StatType.PhysicalDamageBonus,
            ["FIGHT_PROP_FIRE_ADD_HURT"] = StatType.PyroDamageBonus,
            ["FIGHT_PROP_ELEC_ADD_HURT"] = StatType.ElectroDamageBonus,
            ["FIGHT_PROP_WATER_ADD_HURT"] = StatType.HydroDamageBonus,
            ["FIGHT_PROP_GRASS_ADD_HURT"] = StatType.DendroDamageBonus,
            ["FIGHT_PROP_WIND_ADD_HURT"] = StatType.AnemoDamageBonus,
            ["FIGHT_PROP_ROCK_ADD_HURT"] = StatType.GeoDamageBonus,
            ["FIGHT_PROP_ICE_ADD_HURT"] = StatType.CryoDamageBonus,
        };

    /// <summary>Maps Enka artifact <c>equipType</c> strings to <see cref="ArtifactSlot"/>.</summary>
    public static IReadOnlyDictionary<string, ArtifactSlot> EquipTypeToSlot { get; } =
        new Dictionary<string, ArtifactSlot>(StringComparer.Ordinal)
        {
            ["EQUIP_BRACER"] = ArtifactSlot.Flower,
            ["EQUIP_NECKLACE"] = ArtifactSlot.Plume,
            ["EQUIP_SHOES"] = ArtifactSlot.Sands,
            ["EQUIP_RING"] = ArtifactSlot.Goblet,
            ["EQUIP_DRESS"] = ArtifactSlot.Circlet,
        };

    /// <summary>Stats whose <c>flat</c> value is a percentage expressed in points and must be divided by 100.</summary>
    private static readonly HashSet<StatType> PercentStats =
    [
        StatType.HpPercent,
        StatType.AtkPercent,
        StatType.DefPercent,
        StatType.CritRate,
        StatType.CritDamage,
        StatType.EnergyRecharge,
        StatType.HealingBonus,
        StatType.IncomingHealingBonus,
        StatType.PhysicalDamageBonus,
        StatType.PyroDamageBonus,
        StatType.HydroDamageBonus,
        StatType.DendroDamageBonus,
        StatType.ElectroDamageBonus,
        StatType.AnemoDamageBonus,
        StatType.CryoDamageBonus,
        StatType.GeoDamageBonus,
    ];

    /// <summary>Returns <see langword="true"/> when the stat is a percentage stat.</summary>
    /// <param name="type">The stat to classify.</param>
    public static bool IsPercent(StatType type) => PercentStats.Contains(type);

    /// <summary>
    /// Resolves an Enka appendProp id and its raw <c>flat</c> value into a <see cref="StatType"/> and a
    /// normalised value (fractions for percentage stats, e.g. 21.8 -&gt; 0.218).
    /// </summary>
    /// <param name="appendPropId">The Enka appendProp/mainProp id.</param>
    /// <param name="rawValue">The raw value from the <c>flat</c> block.</param>
    /// <returns>The stat type and normalised value; <see cref="StatType.None"/> when unrecognised.</returns>
    public static (StatType Type, double Value) ResolveFlatStat(string? appendPropId, double rawValue)
    {
        if (appendPropId is null || !AppendPropToStatType.TryGetValue(appendPropId, out StatType type))
        {
            return (StatType.None, 0d);
        }

        double value = IsPercent(type) ? rawValue / PercentToFraction : rawValue;
        return (type, value);
    }

    /// <summary>Infers a <see cref="WeaponType"/> from an Enka weapon icon path (e.g. <c>UI_EquipIcon_Sword_Widsith</c>).</summary>
    /// <param name="icon">The icon path from the weapon's <c>flat</c> block.</param>
    /// <returns>The weapon type, or <see cref="WeaponType.Unknown"/> when it cannot be inferred.</returns>
    public static WeaponType InferWeaponType(string? icon)
    {
        if (string.IsNullOrEmpty(icon))
        {
            return WeaponType.Unknown;
        }

        if (icon.Contains("_Sword_", StringComparison.Ordinal))
        {
            return WeaponType.Sword;
        }

        if (icon.Contains("_Claymore_", StringComparison.Ordinal))
        {
            return WeaponType.Claymore;
        }

        if (icon.Contains("_Pole_", StringComparison.Ordinal))
        {
            return WeaponType.Polearm;
        }

        if (icon.Contains("_Bow_", StringComparison.Ordinal))
        {
            return WeaponType.Bow;
        }

        if (icon.Contains("_Catalyst_", StringComparison.Ordinal))
        {
            return WeaponType.Catalyst;
        }

        return WeaponType.Unknown;
    }
}
