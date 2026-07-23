using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="ICharacterAnalyzer"/>: derives progression, per-aspect ratings, crit/ER/EM balance
/// and build efficiency from a character's imported stats and gear. All coefficients live in
/// <see cref="Configuration"/> classes; this type only combines them.
/// </summary>
public sealed class CharacterAnalyzer : ICharacterAnalyzer
{
    private const double PercentScale = 100d;

    private readonly IGameMetadataProvider _metadata;
    private readonly IArtifactAnalyzer _artifactAnalyzer;
    private readonly IWeaponAnalyzer _weaponAnalyzer;
    private readonly ICharacterInsightAnalyzer _insightAnalyzer;

    /// <summary>Initializes the analyzer.</summary>
    /// <param name="metadata">The game metadata provider, used to identify talents.</param>
    /// <param name="artifactAnalyzer">The per-artifact analyzer.</param>
    /// <param name="weaponAnalyzer">The weapon-ranking analyzer.</param>
    /// <param name="insightAnalyzer">The qualitative insight analyzer.</param>
    public CharacterAnalyzer(
        IGameMetadataProvider metadata,
        IArtifactAnalyzer artifactAnalyzer,
        IWeaponAnalyzer weaponAnalyzer,
        ICharacterInsightAnalyzer insightAnalyzer)
    {
        _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        _artifactAnalyzer = artifactAnalyzer ?? throw new ArgumentNullException(nameof(artifactAnalyzer));
        _weaponAnalyzer = weaponAnalyzer ?? throw new ArgumentNullException(nameof(weaponAnalyzer));
        _insightAnalyzer = insightAnalyzer ?? throw new ArgumentNullException(nameof(insightAnalyzer));
    }

    /// <inheritdoc />
    public CharacterAnalysis Analyze(Character character)
    {
        ArgumentNullException.ThrowIfNull(character);

        TalentLevels? talents = IdentifyTalents(character);
        IReadOnlyList<int> talentLevels = ResolveTalentLevels(character, talents);

        List<ArtifactAnalysis> artifactAnalyses = character.Artifacts
            .Select(artifact => _artifactAnalyzer.Analyze(artifact))
            .ToList();

        Rating talentRating = RateTalents(talentLevels);
        Rating weaponRating = RateWeapon(character.Weapon);
        Rating artifactRating = RateArtifacts(character.Artifacts, artifactAnalyses);
        Rating buildRating = RateBuild(character, talentRating, weaponRating, artifactRating);

        var analysis = new CharacterAnalysis
        {
            CharacterId = character.Id,
            Name = character.Name,
            Element = character.Element,
            Level = character.Level,
            MaxLevel = ProgressionConstants.MaxLevel,
            ConstellationLevel = character.ConstellationLevel,
            Talents = talents,
            TalentRating = talentRating,
            WeaponRating = weaponRating,
            ArtifactRating = artifactRating,
            BuildRating = buildRating,
            OverallScore = buildRating.Score,
            CritBalance = AnalyzeCrit(character),
            EnergyRecharge = character.Stats[StatType.EnergyRecharge],
            ElementalMastery = character.Stats[StatType.ElementalMastery],
            Efficiency = ComputeEfficiency(character, talentLevels),
            Artifacts = artifactAnalyses,
            Weapon = _weaponAnalyzer.Analyze(character),
        };

        CharacterInsights insights = _insightAnalyzer.Analyze(character, analysis);

        return analysis with
        {
            Strengths = insights.Strengths,
            Weaknesses = insights.Weaknesses,
            Recommendations = insights.Recommendations,
            BestWeapon = insights.BestWeapon,
            BestArtifacts = insights.BestArtifacts,
        };
    }

    private TalentLevels? IdentifyTalents(Character character)
    {
        IReadOnlyList<int>? order = _metadata.GetCharacter(character.Id)?.TalentSkillIds;
        if (order is not { Count: 3 })
        {
            return null;
        }

        Dictionary<int, int> levelBySkill = character.Talents.ToDictionary(t => t.SkillId, t => t.Level);
        if (!order.All(levelBySkill.ContainsKey))
        {
            return null;
        }

        return new TalentLevels(levelBySkill[order[0]], levelBySkill[order[1]], levelBySkill[order[2]]);
    }

    private static IReadOnlyList<int> ResolveTalentLevels(Character character, TalentLevels? identified)
    {
        if (identified is { } t)
        {
            return [t.NormalAttack, t.ElementalSkill, t.ElementalBurst];
        }

        // Fallback: rate every reported talent when the three main ones cannot be identified.
        return character.Talents.Select(talent => talent.Level).ToList();
    }

    private static Rating RateTalents(IReadOnlyList<int> talentLevels)
    {
        if (talentLevels.Count == 0)
        {
            return RatingScale.ToRating(0d);
        }

        double average = talentLevels.Average();
        double score = average / ProgressionConstants.MaxTalentLevel * PercentScale;
        return RatingScale.ToRating(score);
    }

    private static Rating RateWeapon(Weapon? weapon)
    {
        if (weapon is null)
        {
            return RatingScale.ToRating(0d);
        }

        double level = (double)weapon.Level / ProgressionConstants.MaxLevel;
        double rarity = (double)weapon.Rarity / ProgressionConstants.MaxRarity;
        double refinement = (double)weapon.Refinement / ProgressionConstants.MaxRefinement;

        double score = (ScoringWeights.Weapon.Level * level
            + ScoringWeights.Weapon.Rarity * rarity
            + ScoringWeights.Weapon.Refinement * refinement) * PercentScale;

        return RatingScale.ToRating(score);
    }

    private static Rating RateArtifacts(
        IReadOnlyList<Artifact> artifacts,
        IReadOnlyList<ArtifactAnalysis> analyses)
    {
        if (artifacts.Count == 0)
        {
            return RatingScale.ToRating(0d);
        }

        double averageLevel = artifacts.Average(a => a.Level) / ProgressionConstants.MaxArtifactLevel;
        double averageRarity = artifacts.Average(a => a.Rarity) / ProgressionConstants.MaxRarity;
        double critValue = analyses.Sum(a => a.CritValue);
        double critComponent = Math.Min(1d, critValue / ScoringWeights.Artifact.TargetCritValue);

        double score = (ScoringWeights.Artifact.Level * averageLevel
            + ScoringWeights.Artifact.Rarity * averageRarity
            + ScoringWeights.Artifact.CritValue * critComponent) * PercentScale;

        return RatingScale.ToRating(score);
    }

    private static Rating RateBuild(
        Character character,
        Rating talentRating,
        Rating weaponRating,
        Rating artifactRating)
    {
        double levelScore = (double)character.Level / ProgressionConstants.MaxLevel * PercentScale;

        double score = ScoringWeights.Build.Talents * talentRating.Score
            + ScoringWeights.Build.Weapon * weaponRating.Score
            + ScoringWeights.Build.Artifacts * artifactRating.Score
            + ScoringWeights.Build.Level * levelScore;

        return RatingScale.ToRating(score);
    }

    private static CritBalance AnalyzeCrit(Character character)
    {
        double critRate = character.Stats[StatType.CritRate] * PercentScale;
        double critDamage = character.Stats[StatType.CritDamage] * PercentScale;
        double critValue = CritConstants.CritValueRateWeight * critRate + critDamage;

        if (critRate < CritConstants.MinAssessableCritRate)
        {
            return new CritBalance(critRate, critDamage, critValue, Ratio: 0d, BalanceScore: 0d, IsBalanced: false);
        }

        double ratio = critDamage / critRate;
        double deviation = Math.Abs(ratio - CritConstants.IdealRatio);
        double balanceScore = Math.Clamp(1d - deviation / CritConstants.MaxRatioDeviation, 0d, 1d) * PercentScale;
        bool isBalanced = deviation <= CritConstants.RatioTolerance;

        return new CritBalance(critRate, critDamage, critValue, ratio, balanceScore, isBalanced);
    }

    private static double ComputeEfficiency(Character character, IReadOnlyList<int> talentLevels)
    {
        double level = Fraction(character.Level, ProgressionConstants.MaxLevel);
        double talents = talentLevels.Count == 0
            ? 0d
            : Math.Min(1d, talentLevels.Average() / ProgressionConstants.TargetTalentLevel);
        double weapon = character.Weapon is null
            ? 0d
            : Fraction(character.Weapon.Level, ProgressionConstants.MaxLevel);
        double artifacts = ArtifactCompleteness(character.Artifacts);

        double efficiency = ScoringWeights.Efficiency.Level * level
            + ScoringWeights.Efficiency.Talents * talents
            + ScoringWeights.Efficiency.Weapon * weapon
            + ScoringWeights.Efficiency.Artifacts * artifacts;

        return efficiency * PercentScale;
    }

    private static double ArtifactCompleteness(IReadOnlyList<Artifact> artifacts)
    {
        if (artifacts.Count == 0)
        {
            return 0d;
        }

        double levelCompleteness = artifacts.Average(a => a.Level) / ProgressionConstants.MaxArtifactLevel;
        double slotCompleteness = Fraction(artifacts.Count, ProgressionConstants.FullArtifactSet);
        return Math.Min(1d, levelCompleteness) * slotCompleteness;
    }

    private static double Fraction(int value, int max) => Math.Min(1d, (double)value / max);
}
