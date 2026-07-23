using System.Globalization;
using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="ICharacterInsightAnalyzer"/>: applies threshold-based rules to a character's
/// computed analysis to produce strengths, weaknesses, prioritized recommendations and gear guidance.
/// All thresholds live in <see cref="InsightThresholds"/>.
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

    private static List<string> BuildStrengths(CharacterAnalysis m)
    {
        var strengths = new List<string>();

        if (m.BuildRating.Score >= InsightThresholds.StrongBuildScore)
        {
            strengths.Add(Inv($"Strong overall build (score {m.BuildRating.Score:F0}, tier {m.BuildRating.Tier})"));
        }

        if (m.Efficiency >= InsightThresholds.HighEfficiency)
        {
            strengths.Add("Fully invested (level, talents, weapon and artifacts near max)");
        }

        if (m.Talents is { } t
            && t.NormalAttack >= InsightThresholds.MaxedTalentLevel
            && t.ElementalSkill >= InsightThresholds.MaxedTalentLevel
            && t.ElementalBurst >= InsightThresholds.MaxedTalentLevel)
        {
            strengths.Add(Inv($"Maxed talents ({t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})"));
        }

        if (IsCritInvested(m) && m.CritBalance.IsBalanced)
        {
            strengths.Add(Inv($"Excellent crit balance (CV {m.CritBalance.CritValue:F0}, ratio {m.CritBalance.Ratio:F1})"));
        }

        if (m.Weapon is { } w && w.DpsLossVsBis <= InsightThresholds.NearBisDpsLoss)
        {
            strengths.Add("Near-best-in-slot weapon");
        }

        double artifactEfficiency = AverageArtifactEfficiency(m);
        if (artifactEfficiency >= InsightThresholds.HighArtifactEfficiency)
        {
            strengths.Add(Inv($"High-quality artifacts (avg roll efficiency {artifactEfficiency:F0}%)"));
        }

        return strengths;
    }

    private static List<string> BuildWeaknesses(Character character, CharacterAnalysis m)
    {
        var weaknesses = new List<string>();

        if (m.Level < m.MaxLevel)
        {
            weaknesses.Add(Inv($"Below max level ({m.Level}/{m.MaxLevel})"));
        }

        if (m.Talents is { } t && IsBelowTargetTalents(t))
        {
            weaknesses.Add(Inv($"Talents under-leveled ({t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})"));
        }

        if (m.Weapon is { } w && w.DpsLossVsBis >= InsightThresholds.HighDpsLoss)
        {
            weaknesses.Add(Inv($"Weapon well below best-in-slot ({w.DpsLossVsBis:F0}% loss)"));
        }

        double deadRolls = TotalDeadRolls(m);
        if (deadRolls >= InsightThresholds.DeadRollWarnThreshold)
        {
            weaknesses.Add(Inv($"Wasted artifact rolls (~{deadRolls:F1} in useless substats)"));
        }

        double artifactEfficiency = AverageArtifactEfficiency(m);
        if (m.Artifacts.Count > 0 && artifactEfficiency < InsightThresholds.LowArtifactEfficiency)
        {
            weaknesses.Add(Inv($"Low artifact roll quality ({artifactEfficiency:F0}% avg efficiency)"));
        }

        if (m.EnergyRecharge < InsightThresholds.MinEnergyRecharge)
        {
            weaknesses.Add(Inv($"Low Energy Recharge ({m.EnergyRecharge * PercentScale:F0}%)"));
        }

        if (IsCritInvested(m) && !m.CritBalance.IsBalanced)
        {
            weaknesses.Add(Inv(
                $"Crit ratio off ({m.CritBalance.CritRate:F0} : {m.CritBalance.CritDamage:F0}); aim for CD ≈ 2× CR"));
        }

        if (HasSuboptimalGoblet(character, out StatType gobletMain))
        {
            weaknesses.Add(Inv($"Goblet main stat may be suboptimal (has {gobletMain})"));
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
            recommendations.Add(new Recommendation(
                "level",
                "Raise character level",
                Inv($"Level {m.Level} → {m.MaxLevel} to unlock ascension stats."),
                m.Level < m.MaxLevel - ProgressionConstants.MaxLevel / 10 ? RecommendationPriority.High : RecommendationPriority.Medium));
        }

        if (m.Weapon is { } w && w.DpsLossVsBis >= InsightThresholds.HighDpsLoss && bestWeapon is { } bis)
        {
            recommendations.Add(new Recommendation(
                "weapon",
                "Upgrade weapon",
                Inv($"Equipped weapon is {w.DpsLossVsBis:F0}% below best-in-slot; consider {bis.Name}."),
                RecommendationPriority.High));
        }

        if (HasSuboptimalGoblet(character, out _))
        {
            StatType recommended = ElementDamageBonus.ForElement(character.Element);
            recommendations.Add(new Recommendation(
                "artifacts",
                "Fix goblet main stat",
                Inv($"Use a {recommended} goblet for this character's element."),
                RecommendationPriority.High));
        }

        if (m.Talents is { } t && IsBelowTargetTalents(t))
        {
            recommendations.Add(new Recommendation(
                "talents",
                "Level talents",
                Inv($"Raise talents toward {ProgressionConstants.TargetTalentLevel}+ (currently {t.NormalAttack}/{t.ElementalSkill}/{t.ElementalBurst})."),
                RecommendationPriority.Medium));
        }

        if (m.EnergyRecharge < InsightThresholds.MinEnergyRecharge)
        {
            recommendations.Add(new Recommendation(
                "energy",
                "Increase Energy Recharge",
                Inv($"ER is {m.EnergyRecharge * PercentScale:F0}%; add ER from sands, substats or weapon to reliably burst."),
                RecommendationPriority.Medium));
        }

        if (AverageArtifactEfficiency(m) is var eff and > 0 && eff < InsightThresholds.LowArtifactEfficiency)
        {
            recommendations.Add(new Recommendation(
                "artifacts",
                "Farm higher-quality artifacts",
                Inv($"Average roll efficiency is {eff:F0}%; better substats will raise damage."),
                RecommendationPriority.Medium));
        }

        if (IsCritInvested(m) && !m.CritBalance.IsBalanced)
        {
            recommendations.Add(new Recommendation(
                "crit",
                "Rebalance crit",
                "Adjust artifacts/weapon so CRIT DMG is roughly twice CRIT Rate.",
                RecommendationPriority.Medium));
        }

        if (TotalDeadRolls(m) >= InsightThresholds.DeadRollWarnThreshold)
        {
            recommendations.Add(new Recommendation(
                "artifacts",
                "Replace artifacts with dead rolls",
                "Several rolls went into useless substats; replacing those pieces improves efficiency.",
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

        // Off-element damage goblets and defensive main stats are clearly suboptimal; ATK%/EM are tolerated.
        bool wrongElementBonus = IsDamageBonus(gobletMain) && gobletMain != recommended;
        bool defensiveMain = gobletMain is StatType.HpPercent or StatType.DefPercent;
        return wrongElementBonus || defensiveMain;
    }

    private static bool IsDamageBonus(StatType type) => type is
        StatType.PhysicalDamageBonus or StatType.PyroDamageBonus or StatType.HydroDamageBonus
        or StatType.DendroDamageBonus or StatType.ElectroDamageBonus or StatType.AnemoDamageBonus
        or StatType.CryoDamageBonus or StatType.GeoDamageBonus;

    private static string Inv(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
