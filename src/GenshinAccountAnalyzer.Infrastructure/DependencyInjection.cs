using Microsoft.Extensions.DependencyInjection;

namespace GenshinAccountAnalyzer.Infrastructure;

/// <summary>
/// Composition-root helpers for registering Infrastructure-layer services (implementations of
/// Application ports: persistence, external clients, caching, ...). Kept intentionally small for now;
/// future concerns (account history, remote metadata, PDF export) are registered here.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers the Infrastructure layer's services.</summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // No infrastructure implementations are required yet; the hook exists so hosts can compose
        // all layers uniformly and later additions need no wiring changes at call sites.
        return services;
    }
}
