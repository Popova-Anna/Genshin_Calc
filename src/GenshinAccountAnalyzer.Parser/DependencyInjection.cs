using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Parser.Enka;
using GenshinAccountAnalyzer.Parser.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace GenshinAccountAnalyzer.Parser;

/// <summary>
/// Composition-root helpers for registering parser (importer) services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the game metadata provider and all account importers. Add a new
    /// <see cref="IAccountImporter"/> registration here to support an additional data source.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddParsers(this IServiceCollection services)
    {
        services.AddSingleton<IGameMetadataProvider>(_ => EmbeddedGameMetadataProvider.CreateDefault());
        services.AddScoped<IAccountImporter, EnkaImporter>();

        return services;
    }
}
