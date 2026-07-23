using FluentAssertions;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;
using GenshinAccountAnalyzer.Application.Exceptions;
using GenshinAccountAnalyzer.Domain.Enums;
using GenshinAccountAnalyzer.Domain.Models;

namespace GenshinAccountAnalyzer.Parser.Tests.Application;

public sealed class ImportAccountCommandHandlerTests
{
    private sealed class StubImporter(ImportSource source, Account account) : IAccountImporter
    {
        public ImportSource Source => source;

        public bool WasCalled { get; private set; }

        public Task<Account> ImportAsync(Stream stream, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult(account);
        }
    }

    private static Account EmptyAccount(string uid) => new()
    {
        Uid = uid,
        Profile = new Profile { Nickname = "n" },
        Characters = [],
    };

    [Fact]
    public async Task Handle_SelectsImporterMatchingRequestedSource()
    {
        var enka = new StubImporter(ImportSource.Enka, EmptyAccount("enka"));
        var hoyo = new StubImporter(ImportSource.HoYoLab, EmptyAccount("hoyo"));
        var handler = new ImportAccountCommandHandler([enka, hoyo]);

        using var stream = new MemoryStream();
        Account result = await handler.Handle(
            new ImportAccountCommand(stream, ImportSource.HoYoLab), CancellationToken.None);

        result.Uid.Should().Be("hoyo");
        hoyo.WasCalled.Should().BeTrue();
        enka.WasCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NoImporterForSource_Throws()
    {
        var handler = new ImportAccountCommandHandler([new StubImporter(ImportSource.Enka, EmptyAccount("e"))]);

        using var stream = new MemoryStream();
        Func<Task> act = async () => await handler.Handle(
            new ImportAccountCommand(stream, ImportSource.Akasha), CancellationToken.None);

        await act.Should().ThrowAsync<AccountImportException>();
    }
}
