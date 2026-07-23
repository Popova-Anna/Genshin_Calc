using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="IWeaponAnalyzer"/>: ranks catalog weapons of a character's type by a stat-based
/// score (base ATK plus usefulness-weighted secondary stat) and reports the equipped weapon's loss
/// relative to best-in-slot. The score is a transparent proxy pending the damage calculator.
/// </summary>
public sealed class WeaponAnalyzer : IWeaponAnalyzer
{
    private const double PercentScale = 100d;

    private readonly IWeaponCatalog _catalog;

    /// <summary>Initializes the analyzer with a weapon catalog.</summary>
    /// <param name="catalog">The weapon catalog.</param>
    public WeaponAnalyzer(IWeaponCatalog catalog)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    /// <inheritdoc />
    public WeaponAnalysis Analyze(Character character, SubstatWeightProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(character);
        profile ??= SubstatWeights.DefaultProfile;

        List<(WeaponInfo Weapon, double Score)> ranked = _catalog
            .GetByType(character.WeaponType)
            .Where(weapon => weapon.Rarity >= WeaponScoreWeights.MinRecommendationRarity)
            .Select(weapon => (weapon, Score(weapon, profile)))
            .OrderByDescending(entry => entry.Item2)
            .ToList();

        double bisScore = ranked.Count > 0 ? ranked[0].Score : 0d;

        List<WeaponOption> recommendations = ranked
            .Take(WeaponScoreWeights.RecommendationCount)
            .Select(entry => ToOption(entry.Weapon, entry.Score, bisScore))
            .ToList();

        WeaponOption? equipped = null;
        double dpsLoss = 0d;
        if (character.Weapon is { } weapon)
        {
            WeaponInfo info = _catalog.Get(weapon.Id) ?? FromEquipped(weapon);
            double equippedScore = Score(info, profile);
            equipped = ToOption(info, equippedScore, bisScore);
            dpsLoss = bisScore > 0d ? Math.Max(0d, (bisScore - equippedScore) / bisScore * PercentScale) : 0d;
        }

        return new WeaponAnalysis
        {
            CharacterId = character.Id,
            WeaponType = character.WeaponType,
            Equipped = equipped,
            Recommendations = recommendations,
            DpsLossVsBis = dpsLoss,
            ProfileName = profile.Name,
        };
    }

    private static double Score(WeaponInfo weapon, SubstatWeightProfile profile)
    {
        double attackComponent = weapon.BaseAttack / WeaponScoreWeights.ReferenceBaseAttack;
        double secondaryComponent = SecondaryComponent(weapon, profile);

        return (WeaponScoreWeights.BaseAttackWeight * attackComponent
            + WeaponScoreWeights.SecondaryWeight * secondaryComponent) * PercentScale;
    }

    private static double SecondaryComponent(WeaponInfo weapon, SubstatWeightProfile profile)
    {
        double reference = WeaponScoreWeights.SecondaryReference(weapon.SecondaryStat);
        if (weapon.SecondaryStat == StatType.None || reference <= 0d)
        {
            return 0d;
        }

        return profile[weapon.SecondaryStat] * (weapon.SecondaryValue / reference);
    }

    private static WeaponOption ToOption(WeaponInfo weapon, double score, double bisScore) =>
        new(weapon.Id, weapon.Name, weapon.Rarity, score, bisScore > 0d ? score / bisScore * PercentScale : 0d);

    private static WeaponInfo FromEquipped(Weapon weapon) => new(
        weapon.Id,
        weapon.Name,
        weapon.Type,
        weapon.Rarity,
        weapon.BaseAttack,
        weapon.SecondaryStat?.Type ?? StatType.None,
        weapon.SecondaryStat?.Value ?? 0d);
}
