namespace GenshinAccountAnalyzer.Domain.Models;

/// <summary>
/// A single active talent (skill) level as reported by the data source.
/// Mapping raw skill ids to Normal Attack / Elemental Skill / Elemental Burst requires game metadata
/// and is performed by higher layers; the domain keeps the raw, source-independent representation.
/// </summary>
/// <param name="SkillId">The in-game skill identifier.</param>
/// <param name="Level">The talent level (typically 1-15, including constellation bonuses when the source applies them).</param>
public sealed record Talent(int SkillId, int Level);
