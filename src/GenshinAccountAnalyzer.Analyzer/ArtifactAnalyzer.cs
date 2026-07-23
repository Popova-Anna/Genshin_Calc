using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="IArtifactAnalyzer"/>: derives crit value, roll value, efficiency, dead rolls and
/// per-substat usefulness from an artifact's substats. All coefficients live in <see cref="Configuration"/>.
/// </summary>
public sealed class ArtifactAnalyzer : IArtifactAnalyzer
{
    private const double PercentScale = 100d;

    /// <inheritdoc />
    public ArtifactAnalysis Analyze(Artifact artifact, SubstatWeightProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(artifact);
        profile ??= SubstatWeights.DefaultProfile;

        List<SubstatAnalysis> substats = artifact.SubStats
            .Select(sub => AnalyzeSubstat(sub, profile))
            .ToList();

        double totalRollValue = substats.Sum(s => s.RollValue);
        double deadRolls = substats.Where(s => s.IsDead).Sum(s => s.RollValue);
        int rollCount = artifact.RollCount > 0 ? artifact.RollCount : EstimateRollCount(artifact);

        return new ArtifactAnalysis
        {
            ArtifactId = artifact.Id,
            Slot = artifact.Slot,
            SetName = artifact.SetName,
            Level = artifact.Level,
            Rarity = artifact.Rarity,
            CritValue = CritValue(artifact),
            RollValue = totalRollValue * PercentScale,
            RollCount = rollCount,
            Efficiency = rollCount > 0 ? totalRollValue / rollCount * PercentScale : 0d,
            DeadRolls = deadRolls,
            Substats = substats,
            ProfileName = profile.Name,
        };
    }

    private static SubstatAnalysis AnalyzeSubstat(Stat sub, SubstatWeightProfile profile)
    {
        double maxRoll = SubstatRollData.MaxRoll(sub.Type);
        double rollValue = maxRoll > 0d ? sub.Value / maxRoll : 0d;
        double weight = profile[sub.Type];

        return new SubstatAnalysis(
            Type: sub.Type,
            Value: sub.Value,
            RollValue: rollValue,
            Weight: weight,
            Usefulness: rollValue * weight,
            IsDead: weight <= SubstatWeights.DeadWeightThreshold);
    }

    private static double CritValue(Artifact artifact) =>
        artifact.SubStats.Sum(sub => sub.Type switch
        {
            StatType.CritRate => sub.Value * PercentScale * CritConstants.CritValueRateWeight,
            StatType.CritDamage => sub.Value * PercentScale,
            _ => 0d,
        });

    /// <summary>
    /// Estimates the roll count from level when the source omits per-roll data: a 5★ artifact holds four
    /// substats and gains one roll every four levels.
    /// </summary>
    private static int EstimateRollCount(Artifact artifact) =>
        ProgressionConstants.FullArtifactSet - 1 + artifact.Level / ArtifactRollsPerLevels;

    /// <summary>Levels between substat enhancement rolls.</summary>
    private const int ArtifactRollsPerLevels = 4;
}
