using System.Globalization;
using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Default <see cref="IBuildOptimizer"/>: compares each equipped artifact's main stat against the
/// recommended one, reports the best-in-slot weapon, and scores how optimal the current gear is.
/// Optimising across an inventory is not possible from a showcase export (only equipped pieces are
/// visible), so this optimises main-stat targets rather than searching owned pieces.
/// </summary>
public sealed class BuildOptimizer : IBuildOptimizer
{
    private const double PercentScale = 100d;

    /// <inheritdoc />
    public BuildOptimization Optimize(Character character, CharacterAnalysis analysis)
    {
        ArgumentNullException.ThrowIfNull(character);
        ArgumentNullException.ThrowIfNull(analysis);

        IReadOnlyDictionary<ArtifactSlot, StatType> recommended =
            analysis.BestArtifacts?.MainStats ?? new Dictionary<ArtifactSlot, StatType>();
        Dictionary<ArtifactSlot, Artifact> equipped = character.Artifacts
            .GroupBy(a => a.Slot)
            .ToDictionary(g => g.Key, g => g.First());

        var slots = new List<SlotOptimization>();
        var notes = new List<LocalizedText>();

        foreach (ArtifactSlot slot in SlotMainStats.Slots)
        {
            StatType recommendedMain = recommended.TryGetValue(slot, out StatType r) ? r : StatType.None;
            equipped.TryGetValue(slot, out Artifact? piece);
            StatType currentMain = piece?.MainStat.Type ?? StatType.None;
            bool isOptimal = piece is not null && SlotMainStats.IsAcceptable(slot, currentMain);

            slots.Add(new SlotOptimization(slot, currentMain, recommendedMain, isOptimal));

            if (!isOptimal)
            {
                notes.Add(BuildNote(slot, piece is null, currentMain, recommendedMain));
            }
        }

        int optimalCount = slots.Count(s => s.IsOptimal);
        double score = (double)optimalCount / slots.Count * PercentScale;

        return new BuildOptimization
        {
            CharacterId = character.Id,
            Slots = slots,
            BestWeapon = analysis.BestWeapon,
            OptimizationScore = score,
            Notes = notes,
        };
    }

    private static LocalizedText BuildNote(
        ArtifactSlot slot,
        bool isEmpty,
        StatType currentMain,
        StatType recommendedMain)
    {
        if (isEmpty)
        {
            return new LocalizedText(
                $"Equip a {slot} artifact.",
                $"Наденьте артефакт в слот «{slot}».");
        }

        string target = recommendedMain == StatType.None
            ? Invariant($"an offensive main stat")
            : recommendedMain.ToString();

        return new LocalizedText(
            $"Change {slot} main stat: {currentMain} → {target}.",
            $"Смените основной стат слота «{slot}»: {currentMain} → {target}.");
    }

    private static string Invariant(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
