using GenshinAccountAnalyzer.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenshinAccountAnalyzer.Report;

/// <summary>
/// Composition-root helpers for registering report generation.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers the HTML report generator.</summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddSingleton<IReportGenerator, HtmlReportGenerator>();

        return services;
    }
}
