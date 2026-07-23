using GenshinAccountAnalyzer.Application.Abstractions;

namespace GenshinAccountAnalyzer.Parser.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IGameMetadataProvider"/> for tests, letting each test declare exactly the
/// metadata it expects to flow through the importer. Empty by default (mirrors "no metadata shipped").
/// </summary>
internal sealed class FakeGameMetadataProvider : IGameMetadataProvider
{
    private readonly Dictionary<int, CharacterMetadata> _characters = [];
    private readonly Dictionary<int, WeaponMetadata> _weapons = [];
    private readonly Dictionary<int, string> _sets = [];

    public FakeGameMetadataProvider WithCharacter(int avatarId, CharacterMetadata metadata)
    {
        _characters[avatarId] = metadata;
        return this;
    }

    public FakeGameMetadataProvider WithWeapon(int weaponId, WeaponMetadata metadata)
    {
        _weapons[weaponId] = metadata;
        return this;
    }

    public FakeGameMetadataProvider WithArtifactSet(int setId, string name)
    {
        _sets[setId] = name;
        return this;
    }

    public CharacterMetadata? GetCharacter(int avatarId) =>
        _characters.TryGetValue(avatarId, out CharacterMetadata? value) ? value : null;

    public WeaponMetadata? GetWeapon(int weaponId) =>
        _weapons.TryGetValue(weaponId, out WeaponMetadata? value) ? value : null;

    public string? GetArtifactSetName(int setId) =>
        _sets.TryGetValue(setId, out string? value) ? value : null;
}
