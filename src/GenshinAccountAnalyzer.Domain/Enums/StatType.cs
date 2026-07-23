namespace GenshinAccountAnalyzer.Domain.Enums;

/// <summary>
/// Every character/weapon/artifact statistic the analyzer understands.
/// Values are grouped: base stats, flat/percent offensive stats, crit, utility, and elemental damage bonuses.
/// </summary>
public enum StatType
{
    /// <summary>Unrecognised stat.</summary>
    None = 0,

    /// <summary>Base HP (before percentage bonuses).</summary>
    BaseHp,

    /// <summary>Flat HP.</summary>
    Hp,

    /// <summary>HP%.</summary>
    HpPercent,

    /// <summary>Base ATK (character base + weapon base).</summary>
    BaseAtk,

    /// <summary>Flat ATK.</summary>
    Atk,

    /// <summary>ATK%.</summary>
    AtkPercent,

    /// <summary>Base DEF.</summary>
    BaseDef,

    /// <summary>Flat DEF.</summary>
    Def,

    /// <summary>DEF%.</summary>
    DefPercent,

    /// <summary>Elemental Mastery.</summary>
    ElementalMastery,

    /// <summary>CRIT Rate.</summary>
    CritRate,

    /// <summary>CRIT DMG.</summary>
    CritDamage,

    /// <summary>Energy Recharge.</summary>
    EnergyRecharge,

    /// <summary>Healing Bonus.</summary>
    HealingBonus,

    /// <summary>Incoming Healing Bonus.</summary>
    IncomingHealingBonus,

    /// <summary>Physical DMG Bonus.</summary>
    PhysicalDamageBonus,

    /// <summary>Pyro DMG Bonus.</summary>
    PyroDamageBonus,

    /// <summary>Hydro DMG Bonus.</summary>
    HydroDamageBonus,

    /// <summary>Dendro DMG Bonus.</summary>
    DendroDamageBonus,

    /// <summary>Electro DMG Bonus.</summary>
    ElectroDamageBonus,

    /// <summary>Anemo DMG Bonus.</summary>
    AnemoDamageBonus,

    /// <summary>Cryo DMG Bonus.</summary>
    CryoDamageBonus,

    /// <summary>Geo DMG Bonus.</summary>
    GeoDamageBonus
}
