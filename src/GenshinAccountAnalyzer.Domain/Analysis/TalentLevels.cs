namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// The three main talent levels of a character, once identified from metadata.
/// </summary>
/// <param name="NormalAttack">Normal Attack talent level.</param>
/// <param name="ElementalSkill">Elemental Skill talent level.</param>
/// <param name="ElementalBurst">Elemental Burst talent level.</param>
public readonly record struct TalentLevels(int NormalAttack, int ElementalSkill, int ElementalBurst);
