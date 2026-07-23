using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Analyzes a single artifact: crit value, roll value, roll efficiency, dead rolls and per-substat
/// usefulness, evaluated against a <see cref="SubstatWeightProfile"/>.
/// </summary>
public interface IArtifactAnalyzer
{
    /// <summary>Analyzes <paramref name="artifact"/> using the given weight profile.</summary>
    /// <param name="artifact">The artifact to analyze.</param>
    /// <param name="profile">The usefulness profile; when <see langword="null"/>, a generic default is used.</param>
    /// <returns>The artifact analysis.</returns>
    ArtifactAnalysis Analyze(Artifact artifact, SubstatWeightProfile? profile = null);
}
