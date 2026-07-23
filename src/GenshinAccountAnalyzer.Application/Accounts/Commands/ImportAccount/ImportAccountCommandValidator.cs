using FluentValidation;

namespace GenshinAccountAnalyzer.Application.Accounts.Commands.ImportAccount;

/// <summary>
/// Validates <see cref="ImportAccountCommand"/> before it reaches the handler.
/// </summary>
public sealed class ImportAccountCommandValidator : AbstractValidator<ImportAccountCommand>
{
    /// <summary>Configures the validation rules.</summary>
    public ImportAccountCommandValidator()
    {
        RuleFor(command => command.Content)
            .NotNull().WithMessage("Import content stream is required.")
            .Must(stream => stream.CanRead).WithMessage("Import content stream must be readable.");

        RuleFor(command => command.Source)
            .IsInEnum().WithMessage("Import source is not a recognised value.");
    }
}
