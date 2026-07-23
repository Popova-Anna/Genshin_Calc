namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// An artifact set the character currently has equipped, and how many pieces of it.
/// </summary>
/// <param name="SetId">The in-game set identifier.</param>
/// <param name="SetName">The set display name.</param>
/// <param name="PieceCount">Number of equipped pieces from this set.</param>
public readonly record struct EquippedSet(int SetId, string SetName, int PieceCount);
