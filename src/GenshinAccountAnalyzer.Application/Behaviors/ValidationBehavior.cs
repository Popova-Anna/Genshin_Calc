using FluentValidation;
using MediatR;

namespace GenshinAccountAnalyzer.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs all registered FluentValidation validators for a request
/// before it reaches its handler, aggregating failures into a single <see cref="ValidationException"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>Initializes the behavior with the validators registered for <typeparamref name="TRequest"/>.</summary>
    /// <param name="validators">The validators to run.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>Runs validation, then invokes the next delegate when the request is valid.</summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The next step in the pipeline.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The handler response.</returns>
    /// <exception cref="ValidationException">Thrown when one or more validators fail.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next().ConfigureAwait(false);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next().ConfigureAwait(false);
    }
}
