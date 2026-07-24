using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Configuration;

/// <summary>
/// The main stats considered acceptable for each artifact slot. Flower and Plume are fixed; Sands,
/// Goblet and Circlet accept any of several offensive options, so the optimiser does not over-flag
/// legitimate EM/ER/HP builds.
/// </summary>
public static class SlotMainStats
{
    private static readonly IReadOnlyDictionary<ArtifactSlot, HashSet<StatType>> Acceptable =
        new Dictionary<ArtifactSlot, HashSet<StatType>>
        {
            [ArtifactSlot.Flower] = [StatType.Hp],
            [ArtifactSlot.Plume] = [StatType.Atk],
            [ArtifactSlot.Sands] =
            [
                StatType.AtkPercent, StatType.HpPercent, StatType.DefPercent,
                StatType.EnergyRecharge, StatType.ElementalMastery,
            ],
            [ArtifactSlot.Goblet] =
            [
                StatType.PhysicalDamageBonus, StatType.PyroDamageBonus, StatType.HydroDamageBonus,
                StatType.DendroDamageBonus, StatType.ElectroDamageBonus, StatType.AnemoDamageBonus,
                StatType.CryoDamageBonus, StatType.GeoDamageBonus, StatType.ElementalMastery,
            ],
            [ArtifactSlot.Circlet] =
            [
                StatType.CritRate, StatType.CritDamage, StatType.ElementalMastery, StatType.HealingBonus,
            ],
        };

    /// <summary>The five slots in canonical order.</summary>
    public static IReadOnlyList<ArtifactSlot> Slots { get; } =
    [
        ArtifactSlot.Flower, ArtifactSlot.Plume, ArtifactSlot.Sands,
        ArtifactSlot.Goblet, ArtifactSlot.Circlet,
    ];

    /// <summary>Returns whether a main stat is an acceptable choice for a slot.</summary>
    /// <param name="slot">The artifact slot.</param>
    /// <param name="mainStat">The main stat to test.</param>
    public static bool IsAcceptable(ArtifactSlot slot, StatType mainStat) =>
        Acceptable.TryGetValue(slot, out HashSet<StatType>? set) && set.Contains(mainStat);
}
