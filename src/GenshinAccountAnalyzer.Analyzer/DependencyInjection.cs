using GenshinAccountAnalyzer.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenshinAccountAnalyzer.Analyzer;

/// <summary>
/// Composition-root helpers for registering analyzer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers the character analyzer and related analysis services.</summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddAnalyzers(this IServiceCollection services)
    {
        services.AddScoped<IArtifactAnalyzer, ArtifactAnalyzer>();
        services.AddScoped<IWeaponAnalyzer, WeaponAnalyzer>();
        services.AddScoped<ICharacterInsightAnalyzer, CharacterInsightAnalyzer>();
        services.AddScoped<ITeamAnalyzer, TeamAnalyzer>();
        services.AddScoped<ICharacterAnalyzer, CharacterAnalyzer>();

        return services;
    }
}
