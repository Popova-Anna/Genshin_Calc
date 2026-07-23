using GenshinAccountAnalyzer.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenshinAccountAnalyzer.Calculator;

/// <summary>
/// Composition-root helpers for registering the damage calculator.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers the damage calculator.</summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddCalculator(this IServiceCollection services)
    {
        services.AddSingleton<IDamageCalculator, DamageCalculator>();

        return services;
    }
}
