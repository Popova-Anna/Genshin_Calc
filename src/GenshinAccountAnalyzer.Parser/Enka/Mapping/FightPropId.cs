using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Parser.Enka.Mapping;

/// <summary>
/// Numeric <c>fightPropMap</c> identifiers used by the game engine, and their mapping to <see cref="StatType"/>.
/// Centralised so the parser never contains bare "magic numbers".
/// </summary>
internal static class FightPropId
{
    /// <summary>Base HP (before bonuses).</summary>
    public const int BaseHp = 1;

    /// <summary>Base ATK (character + weapon base).</summary>
    public const int BaseAtk = 4;

    /// <summary>Base DEF.</summary>
    public const int BaseDef = 7;

    /// <summary>CRIT Rate (fraction).</summary>
    public const int CritRate = 20;

    /// <summary>CRIT DMG (fraction).</summary>
    public const int CritDamage = 22;

    /// <summary>Energy Recharge (fraction).</summary>
    public const int EnergyRecharge = 23;

    /// <summary>Healing Bonus (fraction).</summary>
    public const int HealingBonus = 26;

    /// <summary>Incoming Healing Bonus (fraction).</summary>
    public const int IncomingHealingBonus = 27;

    /// <summary>Elemental Mastery (flat).</summary>
    public const int ElementalMastery = 28;

    /// <summary>Physical DMG Bonus (fraction).</summary>
    public const int PhysicalDamageBonus = 30;

    /// <summary>Pyro DMG Bonus (fraction).</summary>
    public const int PyroDamageBonus = 40;

    /// <summary>Electro DMG Bonus (fraction).</summary>
    public const int ElectroDamageBonus = 41;

    /// <summary>Hydro DMG Bonus (fraction).</summary>
    public const int HydroDamageBonus = 42;

    /// <summary>Dendro DMG Bonus (fraction).</summary>
    public const int DendroDamageBonus = 43;

    /// <summary>Anemo DMG Bonus (fraction).</summary>
    public const int AnemoDamageBonus = 44;

    /// <summary>Geo DMG Bonus (fraction).</summary>
    public const int GeoDamageBonus = 45;

    /// <summary>Cryo DMG Bonus (fraction).</summary>
    public const int CryoDamageBonus = 46;

    /// <summary>Final total (max) HP.</summary>
    public const int MaxHp = 2000;

    /// <summary>Final total ATK.</summary>
    public const int MaxAtk = 2001;

    /// <summary>Final total DEF.</summary>
    public const int MaxDef = 2002;

    /// <summary>
    /// Maps each supported numeric <c>fightPropMap</c> id to the <see cref="StatType"/> it represents.
    /// The final-total ids (<see cref="MaxHp"/>/<see cref="MaxAtk"/>/<see cref="MaxDef"/>) populate the
    /// HP/ATK/DEF slots of a character's <see cref="Domain.Common.StatSheet"/>, which by convention holds
    /// final computed totals.
    /// </summary>
    public static IReadOnlyDictionary<int, StatType> ToStatType { get; } = new Dictionary<int, StatType>
    {
        [BaseHp] = StatType.BaseHp,
        [BaseAtk] = StatType.BaseAtk,
        [BaseDef] = StatType.BaseDef,
        [MaxHp] = StatType.Hp,
        [MaxAtk] = StatType.Atk,
        [MaxDef] = StatType.Def,
        [CritRate] = StatType.CritRate,
        [CritDamage] = StatType.CritDamage,
        [EnergyRecharge] = StatType.EnergyRecharge,
        [HealingBonus] = StatType.HealingBonus,
        [IncomingHealingBonus] = StatType.IncomingHealingBonus,
        [ElementalMastery] = StatType.ElementalMastery,
        [PhysicalDamageBonus] = StatType.PhysicalDamageBonus,
        [PyroDamageBonus] = StatType.PyroDamageBonus,
        [ElectroDamageBonus] = StatType.ElectroDamageBonus,
        [HydroDamageBonus] = StatType.HydroDamageBonus,
        [DendroDamageBonus] = StatType.DendroDamageBonus,
        [AnemoDamageBonus] = StatType.AnemoDamageBonus,
        [GeoDamageBonus] = StatType.GeoDamageBonus,
        [CryoDamageBonus] = StatType.CryoDamageBonus,
    };

    /// <summary>The <c>propMap</c> entry type holding the character level.</summary>
    public const int PropTypeLevel = 4001;

    /// <summary>The <c>propMap</c> entry type holding the character ascension phase.</summary>
    public const int PropTypeAscension = 1002;
}
