using Application.Abstractions;

namespace Application.Features.Publishers.Commands;

/// <summary>
/// Publisher olarak kaydol
/// </summary>
public sealed record RegisterAsPublisherCommand(
    long UserId,
    string? CompanyName,
    string? WebsiteUrl,
    string? PromotionMethods,
    string CountryCode = "TR"
) : ICommand<long>;
