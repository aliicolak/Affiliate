using Application.Features.Brands.Commands;
using FluentValidation;

namespace Application.Features.Brands.Validators;

/// <summary>
/// Validator for CreateBrandCommand.
/// </summary>
public sealed class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(100).WithMessage("Brand name must not exceed 100 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must be URL-safe (lowercase letters, numbers, and hyphens only).");
    }
}
