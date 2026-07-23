using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Tests.Support;

/// <summary>In-memory <see cref="IGameMetadataProvider"/> for analyzer tests.</summary>
internal sealed class FakeMetadata : IGameMetadataProvider
{
    private readonly Dictionary<int, CharacterMetadata> _characters = [];

    public FakeMetadata WithCharacter(
        int avatarId,
        string name = "Test",
        ElementType element = ElementType.Anemo,
        WeaponType weapon = WeaponType.Sword,
        int rarity = 5,
        IReadOnlyList<int>? talents = null)
    {
        _characters[avatarId] = new CharacterMetadata(name, element, weapon, rarity)
        {
            TalentSkillIds = talents ?? [],
        };
        return this;
    }

    public CharacterMetadata? GetCharacter(int avatarId) =>
        _characters.TryGetValue(avatarId, out CharacterMetadata? value) ? value : null;

    public WeaponMetadata? GetWeapon(int weaponId) => null;

    public string? GetArtifactSetName(int setId) => null;
}
