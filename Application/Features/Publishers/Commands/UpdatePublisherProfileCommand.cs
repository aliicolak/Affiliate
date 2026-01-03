using Application.Abstractions;

namespace Application.Features.Publishers.Commands;

/// <summary>
/// Publisher profil güncelleme
/// </summary>
public sealed record UpdatePublisherProfileCommand(
    long PublisherId,
    string? CompanyName,
    string? WebsiteUrl,
    string? PromotionMethods,
    string? TaxId,
    string PreferredPaymentMethod,
    string? PaymentDetails
) : ICommand<bool>;
