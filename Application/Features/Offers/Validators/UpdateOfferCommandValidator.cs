using Application.Features.Offers.Commands;
using FluentValidation;

namespace Application.Features.Offers.Validators
{
    public sealed class UpdateOfferCommandValidator : AbstractValidator<UpdateOfferCommand>
    {
        public UpdateOfferCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id 0'dan büyük olmalıdır.");

            RuleFor(x => x.PriceAmount)
                .GreaterThan(0)
                .WithMessage("Fiyat 0'dan büyük olmalıdır.");

            RuleFor(x => x.AffiliateUrl)
                .NotEmpty()
                .WithMessage("Affiliate URL boş olamaz.")
                .Must(BeAValidUrl)
                .WithMessage("Affiliate URL geçerli bir formatta olmalıdır.");

            RuleFor(x => x.LandingUrl)
                .Must(BeAValidUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.LandingUrl))
                .WithMessage("Landing URL geçerli bir formatta olmalıdır.");
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
