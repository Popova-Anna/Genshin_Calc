using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests.Support;

/// <summary>Factory helpers for building domain objects in analyzer tests.</summary>
internal static class Build
{
    public static StatSheet Stats(params (StatType Type, double Value)[] entries) =>
        new(entries.ToDictionary(e => e.Type, e => e.Value));

    public static Weapon Weapon(
        int level = 90,
        int rarity = 5,
        int refinement = 1,
        int ascension = 6,
        WeaponType type = WeaponType.Sword) => new()
    {
        Id = 11503,
        Name = "Test Weapon",
        Type = type,
        Rarity = rarity,
        Level = level,
        Ascension = ascension,
        Refinement = refinement,
        BaseAttack = 500,
    };

    public static Artifact Artifact(
        ArtifactSlot slot,
        int level = 20,
        int rarity = 5,
        double critRateSub = 0d,
        double critDamageSub = 0d)
    {
        var subs = new List<Stat>();
        if (critRateSub > 0)
        {
            subs.Add(new Stat(StatType.CritRate, critRateSub));
        }

        if (critDamageSub > 0)
        {
            subs.Add(new Stat(StatType.CritDamage, critDamageSub));
        }

        return new Artifact
        {
            Id = (int)slot,
            SetId = 15002,
            SetName = "Test Set",
            Slot = slot,
            Rarity = rarity,
            Level = level,
            MainStat = new Stat(StatType.Atk, 311),
            SubStats = subs,
        };
    }

    public static IReadOnlyList<Artifact> FullArtifactSet(
        int level = 20,
        int rarity = 5,
        double critRateSub = 0d,
        double critDamageSub = 0d)
    {
        ArtifactSlot[] slots =
        [
            ArtifactSlot.Flower, ArtifactSlot.Plume, ArtifactSlot.Sands,
            ArtifactSlot.Goblet, ArtifactSlot.Circlet,
        ];
        return slots.Select(slot => Artifact(slot, level, rarity, critRateSub, critDamageSub)).ToList();
    }

    public static Character Character(
        int id = 10000047,
        string name = "Test",
        int level = 90,
        int constellation = 0,
        WeaponType weaponType = WeaponType.Unknown,
        IReadOnlyList<Talent>? talents = null,
        Weapon? weapon = null,
        IReadOnlyList<Artifact>? artifacts = null,
        StatSheet? stats = null) => new()
    {
        Id = id,
        Name = name,
        Level = level,
        ConstellationLevel = constellation,
        WeaponType = weaponType,
        Talents = talents ?? [],
        Weapon = weapon,
        Artifacts = artifacts ?? [],
        Stats = stats ?? StatSheet.Empty,
    };
}
