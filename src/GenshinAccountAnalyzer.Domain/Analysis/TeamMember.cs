using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Analysis;

/// <summary>
/// A character's place in a team: identity, element, build strength and inferred role.
/// </summary>
/// <param name="CharacterId">The in-game character (avatar) identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="Element">The character's element.</param>
/// <param name="BuildScore">The character's overall build score.</param>
/// <param name="Role">Role inferred from build strength within the team (e.g. "Carry", "Support").</param>
public readonly record struct TeamMember(
    int CharacterId,
    string Name,
    ElementType Element,
    double BuildScore,
    string Role);
