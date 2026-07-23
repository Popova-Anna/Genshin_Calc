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
        // A single embedded provider backs both the metadata and weapon-catalog ports.
        services.AddSingleton(_ => EmbeddedGameMetadataProvider.CreateDefault());
        services.AddSingleton<IGameMetadataProvider>(sp => sp.GetRequiredService<EmbeddedGameMetadataProvider>());
        services.AddSingleton<IWeaponCatalog>(sp => sp.GetRequiredService<EmbeddedGameMetadataProvider>());
        services.AddScoped<IAccountImporter, EnkaImporter>();

        return services;
    }
}
