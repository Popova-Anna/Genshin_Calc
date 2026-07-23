using FluentAssertions;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;
using GenshinAccountAnalyzer.Parser.Enka;
using GenshinAccountAnalyzer.Parser.Metadata;
using GenshinAccountAnalyzer.Parser.Tests.TestData;

namespace GenshinAccountAnalyzer.Parser.Tests.Metadata;

public sealed class EmbeddedGameMetadataProviderTests
{
    [Fact]
    public void CreateDefault_ResolvesKnownCharacterFromEmbeddedData()
    {
        IGameMetadataProvider provider = EmbeddedGameMetadataProvider.CreateDefault();

        CharacterMetadata? kazuha = provider.GetCharacter(SampleData.KazuhaAvatarId);

        kazuha.Should().NotBeNull();
        kazuha!.Name.Should().Be("Kaedehara Kazuha");
        kazuha.Element.Should().Be(ElementType.Anemo);
        kazuha.Weapon.Should().Be(WeaponType.Sword);
        kazuha.Rarity.Should().Be(5);
    }

    [Fact]
    public void CreateDefault_UnknownCharacter_ReturnsNull()
    {
        IGameMetadataProvider provider = EmbeddedGameMetadataProvider.CreateDefault();

        provider.GetCharacter(1).Should().BeNull();
    }

    [Fact]
    public async Task ImportSample_WithDefaultMetadata_ProducesHumanReadableCharacters()
    {
        var importer = new EnkaImporter(EmbeddedGameMetadataProvider.CreateDefault());

        await using FileStream sample = SampleData.OpenEnkaSample();
        Account account = await importer.ImportAsync(sample, CancellationToken.None);

        Character kazuha = account.Characters.First(c => c.Id == SampleData.KazuhaAvatarId);
        kazuha.Name.Should().Be("Kaedehara Kazuha");
        kazuha.Element.Should().Be(ElementType.Anemo);
        kazuha.Rarity.Should().Be(5);

        // Every character present in the embedded metadata resolves to a real (non-placeholder) name.
        account.Characters
            .Where(c => EmbeddedGameMetadataProvider.CreateDefault().GetCharacter(c.Id) is not null)
            .Should().OnlyContain(c => !c.Name.StartsWith("Character ", StringComparison.Ordinal));
    }
}
