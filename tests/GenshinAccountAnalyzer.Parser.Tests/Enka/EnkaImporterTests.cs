using System.Text;
using FluentAssertions;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Exceptions;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;
using GenshinAccountAnalyzer.Parser.Enka;
using GenshinAccountAnalyzer.Parser.Tests.Fakes;
using GenshinAccountAnalyzer.Parser.Tests.TestData;

namespace GenshinAccountAnalyzer.Parser.Tests.Enka;

public sealed class EnkaImporterTests
{
    private const double Tolerance = 0.001;

    private static EnkaImporter CreateImporter(IGameMetadataProvider? metadata = null) =>
        new(metadata ?? new FakeGameMetadataProvider());

    private static async Task<Account> ImportSampleAsync(IGameMetadataProvider? metadata = null)
    {
        await using FileStream sample = SampleData.OpenEnkaSample();
        return await CreateImporter(metadata).ImportAsync(sample, CancellationToken.None);
    }

    [Fact]
    public void Source_IsEnka()
    {
        CreateImporter().Source.Should().Be(ImportSource.Enka);
    }

    [Fact]
    public async Task ImportAsync_Sample_MapsUidAndProfile()
    {
        Account account = await ImportSampleAsync();

        account.Uid.Should().Be(SampleData.SampleUid);
        account.Source.Should().Be(ImportSource.Enka);

        account.Profile.Nickname.Should().Be("Ann");
        account.Profile.AdventureRank.Should().Be(60);
        account.Profile.WorldLevel.Should().Be(9);
        account.Profile.Achievements.Should().Be(1316);
        account.Profile.SpiralAbyssFloor.Should().Be(12);
        account.Profile.SpiralAbyssChamber.Should().Be(3);
        account.Profile.ProfilePictureCharacterId.Should().Be(SampleData.KazuhaAvatarId);
    }

    [Fact]
    public async Task ImportAsync_Sample_ReturnsAllCharacters()
    {
        Account account = await ImportSampleAsync();

        account.Characters.Should().HaveCount(12);
        account.Characters.Should().OnlyHaveUniqueItems(character => character.Id);
    }

    [Fact]
    public async Task ImportAsync_Sample_MapsFirstCharacterCoreFields()
    {
        Account account = await ImportSampleAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        kazuha.Level.Should().Be(90);
        kazuha.Ascension.Should().Be(6);
        kazuha.ConstellationLevel.Should().Be(1);
        kazuha.WeaponType.Should().Be(WeaponType.Sword, "the weapon type is inferred from the equipped weapon icon");
        kazuha.Weapon.Should().NotBeNull();
        kazuha.Artifacts.Should().HaveCount(5);
        kazuha.Talents.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ImportAsync_Sample_MapsFinalStats()
    {
        Account account = await ImportSampleAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        kazuha.Stats[StatType.Hp].Should().BeApproximately(20556.22, 0.1);
        kazuha.Stats[StatType.CritRate].Should().BeApproximately(0.3766, Tolerance);
        kazuha.Stats[StatType.CritDamage].Should().BeApproximately(1.8446, Tolerance);
        kazuha.Stats[StatType.EnergyRecharge].Should().BeApproximately(1.2202, Tolerance);
        kazuha.Stats[StatType.ElementalMastery].Should().BeApproximately(577.09, 0.1);
        kazuha.Stats[StatType.AnemoDamageBonus].Should().BeApproximately(0.616, Tolerance);
    }

    [Fact]
    public async Task ImportAsync_Sample_MapsWeapon()
    {
        Account account = await ImportSampleAsync();
        Weapon weapon = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId).Weapon!;

        weapon.Id.Should().Be(11503);
        weapon.Type.Should().Be(WeaponType.Sword);
        weapon.Rarity.Should().Be(5);
        weapon.Level.Should().Be(90);
        weapon.Ascension.Should().Be(6);
        weapon.Refinement.Should().Be(1);
        weapon.BaseAttack.Should().BeApproximately(608, Tolerance);
        weapon.SecondaryStat.Should().NotBeNull();
        weapon.SecondaryStat!.Value.Type.Should().Be(StatType.ElementalMastery);
        weapon.SecondaryStat.Value.Value.Should().BeApproximately(198, Tolerance);
    }

    [Fact]
    public async Task ImportAsync_Sample_MapsFlowerArtifact()
    {
        Account account = await ImportSampleAsync();
        Artifact flower = account.Characters
            .First(c => c.Id == SampleData.KazuhaAvatarId)
            .Artifacts.First(a => a.Slot == ArtifactSlot.Flower);

        flower.Rarity.Should().Be(5);
        flower.Level.Should().Be(20, "Enka stores artifact level one-based (21 -> +20)");
        flower.SetId.Should().Be(15002);
        flower.SetName.Should().Be("Set 15002", "no metadata is supplied, so a placeholder name is used");
        flower.MainStat.Type.Should().Be(StatType.Hp);
        flower.MainStat.Value.Should().BeApproximately(4780, Tolerance);
        flower.SubStats.Should().Contain(s => s.Type == StatType.CritDamage);
        flower.SubStats.First(s => s.Type == StatType.CritDamage).Value
            .Should().BeApproximately(0.218, Tolerance, "percent substats are normalised to fractions");
    }

    [Fact]
    public async Task ImportAsync_WithMetadata_EnrichesNamesElementsAndSets()
    {
        var metadata = new FakeGameMetadataProvider()
            .WithCharacter(
                SampleData.KazuhaAvatarId,
                new CharacterMetadata("Kaedehara Kazuha", ElementType.Anemo, WeaponType.Sword, Rarity: 5))
            .WithArtifactSet(15002, "Viridescent Venerer");

        Account account = await ImportSampleAsync(metadata);
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        kazuha.Name.Should().Be("Kaedehara Kazuha");
        kazuha.Element.Should().Be(ElementType.Anemo);
        kazuha.Rarity.Should().Be(5);
        kazuha.Artifacts.Should().Contain(a => a.SetName == "Viridescent Venerer");
    }

    [Fact]
    public async Task ImportAsync_WithoutMetadata_UsesSafePlaceholders()
    {
        Account account = await ImportSampleAsync();
        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);

        kazuha.Name.Should().Be($"Character {SampleData.KazuhaAvatarId}");
        kazuha.Element.Should().Be(ElementType.Unknown);
    }

    [Fact]
    public async Task ImportAsync_InvalidJson_ThrowsAccountImportException()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{ not valid json "));

        Func<Task> act = async () => await CreateImporter().ImportAsync(stream, CancellationToken.None);

        await act.Should().ThrowAsync<AccountImportException>();
    }

    [Fact]
    public async Task ImportAsync_MissingPlayerInfo_ThrowsAccountImportException()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{ \"avatarInfoList\": [] }"));

        Func<Task> act = async () => await CreateImporter().ImportAsync(stream, CancellationToken.None);

        await act.Should().ThrowAsync<AccountImportException>()
            .WithMessage("*playerInfo*");
    }
}
