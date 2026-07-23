using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer.Tests.Support;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer.Tests;

public sealed class WeaponAnalyzerTests
{
    private const int Mistsplitter = 11509;
    private const int Freedom = 11503;
    private const int Favonius = 11401;
    private const int LowRarity = 11301;

    private static FakeWeaponCatalog SwordCatalog() => new FakeWeaponCatalog()
        .Add(new WeaponInfo(Mistsplitter, "Mistsplitter Reforged", WeaponType.Sword, 5, 674.33, StatType.CritDamage, 0.441))
        .Add(new WeaponInfo(Freedom, "Freedom-Sworn", WeaponType.Sword, 5, 608.07, StatType.ElementalMastery, 198.46))
        .Add(new WeaponInfo(Favonius, "Favonius Sword", WeaponType.Sword, 4, 454.36, StatType.EnergyRecharge, 0.6125))
        .Add(new WeaponInfo(LowRarity, "Cool Steel", WeaponType.Sword, 3, 401d, StatType.AtkPercent, 0.35));

    private static Weapon EquippedWeapon(int id) => new()
    {
        Id = id,
        Name = "Equipped",
        Type = WeaponType.Sword,
        Rarity = 4,
        Level = 90,
        Ascension = 6,
        Refinement = 1,
        BaseAttack = 454.36,
    };

    [Fact]
    public void Analyze_RanksWeaponsByScore_BisFirst()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword, weapon: EquippedWeapon(Favonius));

        WeaponAnalysis analysis = new WeaponAnalyzer(SwordCatalog()).Analyze(character);

        analysis.Recommendations.Should().HaveCount(3);
        analysis.Recommendations[0].Name.Should().Be("Mistsplitter Reforged");
        analysis.Recommendations[1].Name.Should().Be("Freedom-Sworn");
        analysis.Recommendations[0].RelativeToBis.Should().BeApproximately(100d, 0.001);
    }

    [Fact]
    public void Analyze_ExcludesBelowMinimumRarity()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword, weapon: EquippedWeapon(Favonius));

        WeaponAnalysis analysis = new WeaponAnalyzer(SwordCatalog()).Analyze(character);

        analysis.Recommendations.Should().NotContain(o => o.Id == LowRarity);
    }

    [Fact]
    public void Analyze_ComputesDpsLossForEquipped()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword, weapon: EquippedWeapon(Favonius));

        WeaponAnalysis analysis = new WeaponAnalyzer(SwordCatalog()).Analyze(character);

        analysis.Equipped.Should().NotBeNull();
        analysis.Equipped!.Value.Name.Should().Be("Favonius Sword");
        analysis.DpsLossVsBis.Should().BeApproximately(28.0, 0.5);
        analysis.Equipped.Value.RelativeToBis.Should().BeApproximately(72.0, 0.5);
    }

    [Fact]
    public void Analyze_EquippedIsBis_HasNoLoss()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword, weapon: EquippedWeapon(Mistsplitter));

        WeaponAnalysis analysis = new WeaponAnalyzer(SwordCatalog()).Analyze(character);

        analysis.DpsLossVsBis.Should().BeApproximately(0d, 0.001);
        analysis.Equipped!.Value.RelativeToBis.Should().BeApproximately(100d, 0.001);
    }

    [Fact]
    public void Analyze_NoWeapon_LeavesEquippedNull()
    {
        Character character = Build.Character(weaponType: WeaponType.Sword, weapon: null);

        WeaponAnalysis analysis = new WeaponAnalyzer(SwordCatalog()).Analyze(character);

        analysis.Equipped.Should().BeNull();
        analysis.DpsLossVsBis.Should().Be(0d);
        analysis.Recommendations.Should().NotBeEmpty();
    }

    [Fact]
    public void Analyze_TypeWithNoCatalogWeapons_ReturnsEmptyRecommendations()
    {
        Character character = Build.Character(weaponType: WeaponType.Catalyst, weapon: null);

        WeaponAnalysis analysis = new WeaponAnalyzer(SwordCatalog()).Analyze(character);

        analysis.Recommendations.Should().BeEmpty();
        analysis.DpsLossVsBis.Should().Be(0d);
    }
}
