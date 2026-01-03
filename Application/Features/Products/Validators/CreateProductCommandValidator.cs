using Application.Features.Products.Commands;
using FluentValidation;

namespace Application.Features.Products.Validators;

/// <summary>
/// Validator for CreateProductCommand.
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(200).WithMessage("Slug must not exceed 200 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

        RuleFor(x => x.LanguageCode)
            .NotEmpty().WithMessage("Language code is required.")
            .Length(2, 5).WithMessage("Language code must be between 2 and 5 characters.");
    }
}
