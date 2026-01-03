using Application.Abstractions;

namespace Application.Features.VendorApplications.Commands;

/// <summary>
/// Satıcı başvurusu oluşturma
/// </summary>
public sealed record CreateVendorApplicationCommand(
    long UserId,
    string CompanyName,
    string ContactEmail,
    string? Phone,
    string? WebsiteUrl,
    string? Description,
    string CountryCode,
    string? TaxId,
    string? Industry,
    string? EstimatedMonthlySales,
    string? ExistingAffiliateNetworks
) : ICommand<long>;
