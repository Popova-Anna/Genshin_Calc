using System.Reflection;
using FluentValidation;
using GenshinAccountAnalyzer.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GenshinAccountAnalyzer.Application;

/// <summary>
/// Composition-root helpers for registering the Application layer's services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers MediatR handlers, FluentValidation validators and the validation pipeline behavior
    /// for the Application layer.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
