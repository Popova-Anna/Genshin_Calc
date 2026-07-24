using System.Globalization;
using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="ICharacterInsightAnalyzer"/>: applies threshold-based rules to a character's
/// computed analysis to produce bilingual (English/Russian) strengths, weaknesses, prioritized
/// recommendations and gear guidance. All thresholds live in <see cref="InsightThresholds"/>.
/// </summary>
public sealed class CharacterInsightAnalyzer : ICharacterInsightAnalyzer
{
    private const double PercentScale = 100d;

    /// <inheritdoc />
    public CharacterInsights Analyze(Character character, CharacterAnalysis metrics)
    {
        ArgumentNullException.ThrowIfNull(character);
        ArgumentNullException.ThrowIfNull(metrics);

        WeaponOption? bestWeapon = metrics.Weapon is { Recommendations.Count: > 0 } weapon
            ? weapon.Recommendations[0]
            : null;

        return new CharacterInsights
        {
            Strengths = BuildStrengths(metrics),
            Weaknesses = BuildWeaknesses(character, metrics),
            Recommendations = BuildRecommendations(character, metrics, bestWeapon),
            BestWeapon = bestWeapon,
            BestArtifacts = BuildArtifactRecommendation(character),
        };
    }

    private static List<LocalizedText> BuildStrengths(CharacterAnalysis m)
    {
        var strengths = new List<LocalizedText>();

        if (m.BuildRating.Score >= InsightThresholds.StrongBuildScore)
        {
            strengths.Add(L(
                $"Strong overall build (score {F(m.BuildRating.Score, 0)}, tier {m.BuildRating.Tier})",
                $"Крепкий билд (счёт {F(m.BuildRating.Score, 0)}, тир {m.BuildRating.Tier})"));
        }

        if (m.Efficiency >= InsightThresholds.HighEfficiency)
        {
            strengths.Add(L(
                "Fully invested (level, talents, weapon and artifacts near max)",
                "Полностью прокачан (уровень, таланты, оружие и артефакты почти на максимуме)"));
        }

        if (m.Talents is { } t
            && t.NormalAttack >= InsightThresholds.MaxedTalentLevel
            && t.ElementalSkill >= InsightThresholds.MaxedTalentLevel
            && t.ElementalBurst >= InsightThresholds.MaxedTalentLevel)
        {
            strengths.Add(L(
                $"Maxed talents ({t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})",
                $"Максимальные таланты ({t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})"));
        }

        if (IsCritInvested(m) && m.CritBalance.IsBalanced)
        {
            strengths.Add(L(
                $"Excellent crit balance (CV {F(m.CritBalance.CritValue, 0)}, ratio {F(m.CritBalance.Ratio, 1)})",
                $"Отличный баланс критов (CV {F(m.CritBalance.CritValue, 0)}, соотн. {F(m.CritBalance.Ratio, 1)})"));
        }

        if (m.Weapon is { } w && w.DpsLossVsBis <= InsightThresholds.NearBisDpsLoss)
        {
            strengths.Add(L("Near-best-in-slot weapon", "Оружие близко к лучшему (BiS)"));
        }

        double artifactEfficiency = AverageArtifactEfficiency(m);
        if (artifactEfficiency >= InsightThresholds.HighArtifactEfficiency)
        {
            strengths.Add(L(
                $"High-quality artifacts (avg roll efficiency {F(artifactEfficiency, 0)}%)",
                $"Качественные артефакты (средняя эффективность роллов {F(artifactEfficiency, 0)}%)"));
        }

        return strengths;
    }

    private static List<LocalizedText> BuildWeaknesses(Character character, CharacterAnalysis m)
    {
        var weaknesses = new List<LocalizedText>();

        if (m.Level < m.MaxLevel)
        {
            weaknesses.Add(L(
                $"Below max level ({m.Level}/{m.MaxLevel})",
                $"Не максимальный уровень ({m.Level}/{m.MaxLevel})"));
        }

        if (m.Talents is { } t && IsBelowTargetTalents(t))
        {
            weaknesses.Add(L(
                $"Talents under-leveled ({t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})",
                $"Таланты недокачаны ({t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})"));
        }

        if (m.Weapon is { } w && w.DpsLossVsBis >= InsightThresholds.HighDpsLoss)
        {
            weaknesses.Add(L(
                $"Weapon well below best-in-slot ({F(w.DpsLossVsBis, 0)}% loss)",
                $"Оружие сильно ниже BiS (потеря {F(w.DpsLossVsBis, 0)}%)"));
        }

        double deadRolls = TotalDeadRolls(m);
        if (deadRolls >= InsightThresholds.DeadRollWarnThreshold)
        {
            weaknesses.Add(L(
                $"Wasted artifact rolls (~{F(deadRolls, 1)} in useless substats)",
                $"Потерянные роллы артефактов (~{F(deadRolls, 1)} в бесполезных сабстатах)"));
        }

        double artifactEfficiency = AverageArtifactEfficiency(m);
        if (m.Artifacts.Count > 0 && artifactEfficiency < InsightThresholds.LowArtifactEfficiency)
        {
            weaknesses.Add(L(
                $"Low artifact roll quality ({F(artifactEfficiency, 0)}% avg efficiency)",
                $"Низкое качество роллов артефактов (средняя эффективность {F(artifactEfficiency, 0)}%)"));
        }

        if (m.EnergyRecharge < InsightThresholds.MinEnergyRecharge)
        {
            weaknesses.Add(L(
                $"Low Energy Recharge ({F(m.EnergyRecharge * PercentScale, 0)}%)",
                $"Низкое восстановление энергии ({F(m.EnergyRecharge * PercentScale, 0)}%)"));
        }

        if (IsCritInvested(m) && !m.CritBalance.IsBalanced)
        {
            weaknesses.Add(L(
                $"Crit ratio off ({F(m.CritBalance.CritRate, 0)} : {F(m.CritBalance.CritDamage, 0)}); aim for CD ≈ 2× CR",
                $"Дисбаланс критов ({F(m.CritBalance.CritRate, 0)} : {F(m.CritBalance.CritDamage, 0)}); цель — крит.урон ≈ 2× крит.шанс"));
        }

        if (HasSuboptimalGoblet(character, out StatType gobletMain))
        {
            weaknesses.Add(L(
                $"Goblet main stat may be suboptimal (has {gobletMain})",
                $"Основной стат кубка не оптимален ({gobletMain})"));
        }

        return weaknesses;
    }

    private static List<Recommendation> BuildRecommendations(
        Character character,
        CharacterAnalysis m,
        WeaponOption? bestWeapon)
    {
        var recommendations = new List<Recommendation>();

        if (m.Level < m.MaxLevel)
        {
            RecommendationPriority priority = m.Level < m.MaxLevel - ProgressionConstants.MaxLevel / 10
                ? RecommendationPriority.High
                : RecommendationPriority.Medium;
            recommendations.Add(new Recommendation("level",
                L("Raise character level", "Поднять уровень персонажа"),
                L($"Level {m.Level} → {m.MaxLevel} to unlock ascension stats.",
                  $"Уровень {m.Level} → {m.MaxLevel} откроет статы возвышения."),
                priority));
        }

        if (m.Weapon is { } w && w.DpsLossVsBis >= InsightThresholds.HighDpsLoss && bestWeapon is { } bis)
        {
            recommendations.Add(new Recommendation("weapon",
                L("Upgrade weapon", "Улучшить оружие"),
                L($"Equipped weapon is {F(w.DpsLossVsBis, 0)}% below best-in-slot; consider {bis.Name}.",
                  $"Оружие на {F(w.DpsLossVsBis, 0)}% ниже BiS; рассмотрите {bis.Name}."),
                RecommendationPriority.High));
        }

        if (HasSuboptimalGoblet(character, out _))
        {
            StatType recommended = ElementDamageBonus.ForElement(character.Element);
            recommendations.Add(new Recommendation("artifacts",
                L("Fix goblet main stat", "Исправить основной стат кубка"),
                L($"Use a {recommended} goblet for this character's element.",
                  $"Используйте кубок «{recommended}» под стихию персонажа."),
                RecommendationPriority.High));
        }

        if (m.Talents is { } t && IsBelowTargetTalents(t))
        {
            recommendations.Add(new Recommendation("talents",
                L("Level talents", "Прокачать таланты"),
                L($"Raise talents toward {ProgressionConstants.TargetTalentLevel}+ (currently {t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst}).",
                  $"Поднимите таланты до {ProgressionConstants.TargetTalentLevel}+ (сейчас {t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})."),
                RecommendationPriority.Medium));
        }

        if (m.EnergyRecharge < InsightThresholds.MinEnergyRecharge)
        {
            recommendations.Add(new Recommendation("energy",
                L("Increase Energy Recharge", "Повысить восстановление энергии"),
                L($"ER is {F(m.EnergyRecharge * PercentScale, 0)}%; add ER from sands, substats or weapon to reliably burst.",
                  $"ВЭ {F(m.EnergyRecharge * PercentScale, 0)}%; добавьте ВЭ через часы, сабстаты или оружие для стабильного взрыва."),
                RecommendationPriority.Medium));
        }

        double eff = AverageArtifactEfficiency(m);
        if (eff > 0 && eff < InsightThresholds.LowArtifactEfficiency)
        {
            recommendations.Add(new Recommendation("artifacts",
                L("Farm higher-quality artifacts", "Фармить более качественные артефакты"),
                L($"Average roll efficiency is {F(eff, 0)}%; better substats will raise damage.",
                  $"Средняя эффективность роллов {F(eff, 0)}%; лучшие сабстаты повысят урон."),
                RecommendationPriority.Medium));
        }

        if (IsCritInvested(m) && !m.CritBalance.IsBalanced)
        {
            recommendations.Add(new Recommendation("crit",
                L("Rebalance crit", "Сбалансировать криты"),
                L("Adjust artifacts/weapon so CRIT DMG is roughly twice CRIT Rate.",
                  "Настройте артефакты/оружие так, чтобы крит.урон был примерно вдвое больше крит.шанса."),
                RecommendationPriority.Medium));
        }

        if (TotalDeadRolls(m) >= InsightThresholds.DeadRollWarnThreshold)
        {
            recommendations.Add(new Recommendation("artifacts",
                L("Replace artifacts with dead rolls", "Заменить артефакты с мёртвыми роллами"),
                L("Several rolls went into useless substats; replacing those pieces improves efficiency.",
                  "Несколько роллов ушли в бесполезные сабстаты; замена этих предметов повысит эффективность."),
                RecommendationPriority.Low));
        }

        return recommendations
            .OrderByDescending(recommendation => recommendation.Priority)
            .ToList();
    }

    private static ArtifactRecommendation BuildArtifactRecommendation(Character character)
    {
        var mainStats = new Dictionary<ArtifactSlot, StatType>
        {
            [ArtifactSlot.Flower] = StatType.Hp,
            [ArtifactSlot.Plume] = StatType.Atk,
            [ArtifactSlot.Sands] = StatType.AtkPercent,
            [ArtifactSlot.Circlet] = StatType.CritRate,
        };

        StatType goblet = ElementDamageBonus.ForElement(character.Element);
        if (goblet != StatType.None)
        {
            mainStats[ArtifactSlot.Goblet] = goblet;
        }

        List<EquippedSet> currentSets = character.Artifacts
            .Where(a => a.SetId != 0)
            .GroupBy(a => (a.SetId, a.SetName))
            .Select(group => new EquippedSet(group.Key.SetId, group.Key.SetName, group.Count()))
            .Where(set => set.PieceCount >= 2)
            .OrderByDescending(set => set.PieceCount)
            .ToList();

        return new ArtifactRecommendation
        {
            MainStats = mainStats,
            Substats = [StatType.CritRate, StatType.CritDamage, StatType.AtkPercent, StatType.ElementalMastery, StatType.EnergyRecharge],
            CurrentSets = currentSets,
            Notes = "Generic element-based guidance; role-specific recommendations pending a build dataset.",
        };
    }

    private static bool IsCritInvested(CharacterAnalysis m) =>
        m.CritBalance.CritValue >= InsightThresholds.CritInvestmentFloor;

    private static bool IsBelowTargetTalents(TalentLevels t) =>
        t.NormalAttack < ProgressionConstants.TargetTalentLevel
        || t.ElementalSkill < ProgressionConstants.TargetTalentLevel
        || t.ElementalBurst < ProgressionConstants.TargetTalentLevel;

    private static double TotalDeadRolls(CharacterAnalysis m) => m.Artifacts.Sum(a => a.DeadRolls);

    private static double AverageArtifactEfficiency(CharacterAnalysis m) =>
        m.Artifacts.Count == 0 ? 0d : m.Artifacts.Average(a => a.Efficiency);

    private static bool HasSuboptimalGoblet(Character character, out StatType gobletMain)
    {
        gobletMain = StatType.None;
        StatType recommended = ElementDamageBonus.ForElement(character.Element);
        if (recommended == StatType.None)
        {
            return false;
        }

        Artifact? goblet = character.Artifacts.FirstOrDefault(a => a.Slot == ArtifactSlot.Goblet);
        if (goblet is null)
        {
            return false;
        }

        gobletMain = goblet.MainStat.Type;

        bool wrongElementBonus = IsDamageBonus(gobletMain) && gobletMain != recommended;
        bool defensiveMain = gobletMain is StatType.HpPercent or StatType.DefPercent;
        return wrongElementBonus || defensiveMain;
    }

    private static bool IsDamageBonus(StatType type) => type is
        StatType.PhysicalDamageBonus or StatType.PyroDamageBonus or StatType.HydroDamageBonus
        or StatType.DendroDamageBonus or StatType.ElectroDamageBonus or StatType.AnemoDamageBonus
        or StatType.CryoDamageBonus or StatType.GeoDamageBonus;

    private static LocalizedText L(FormattableString en, FormattableString ru) =>
        new(en.ToString(CultureInfo.InvariantCulture), ru.ToString(CultureInfo.InvariantCulture));

    private static LocalizedText L(string en, string ru) => new(en, ru);

    private static string F(double value, int digits) =>
        value.ToString("F" + digits.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
}
