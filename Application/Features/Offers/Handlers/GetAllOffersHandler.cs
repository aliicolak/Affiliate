using Application.Abstractions.Persistence;
using Application.Common.Responses;
using Application.Features.Offers.DTOs;
using Application.Features.Offers.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Offers.Handlers
{
    public sealed class GetAllOffersHandler
        : IRequestHandler<GetAllOffersQuery, PagedResponse<OfferListResponseDto>>
    {
        private readonly IAppDbContext _context;

        public GetAllOffersHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<OfferListResponseDto>> Handle(GetAllOffersQuery request, CancellationToken cancellationToken)
        {
            var dto = request.RequestDto;

            var query = _context.Offers
                .AsNoTracking()
                .Include(o => o.Merchant)
                .Include(o => o.Program)
                .AsQueryable();

            // === Filtreler ===
            if (dto.MerchantId is not null)
                query = query.Where(o => o.MerchantId == dto.MerchantId);

            if (dto.ProgramId is not null)
                query = query.Where(o => o.ProgramId == dto.ProgramId);

            if (dto.InStock is not null)
                query = query.Where(o => o.InStock == dto.InStock);

            if (dto.PriceMin is not null)
                query = query.Where(o => o.PriceAmount >= dto.PriceMin);

            if (dto.PriceMax is not null)
                query = query.Where(o => o.PriceAmount <= dto.PriceMax);

            if (!string.IsNullOrWhiteSpace(dto.Search))
                query = query.Where(o =>
                    o.Merchant.Name.Contains(dto.Search) ||
                    (o.Program != null && o.Program.Name.Contains(dto.Search)));

            // === Toplam kayıt sayısı ===
            var totalCount = await query.CountAsync(cancellationToken);

            // === Sıralama ===
            query = dto.SortOrder?.ToLower() == "desc"
                ? dto.SortBy?.ToLower() switch
                {
                    "priceamount" => query.OrderByDescending(o => o.PriceAmount),
                    "createdutc" => query.OrderByDescending(o => o.CreatedUtc),
                    _ => query.OrderByDescending(o => o.Id)
                }
                : dto.SortBy?.ToLower() switch
                {
                    "priceamount" => query.OrderBy(o => o.PriceAmount),
                    "createdutc" => query.OrderBy(o => o.CreatedUtc),
                    _ => query.OrderBy(o => o.Id)
                };

            // === Sayfalama ===
            query = query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize);

            // === Projection (Entity → DTO) ===
            var items = await query
                .Select(o => new OfferListResponseDto
                {
                    Id = o.Id,
                    MerchantName = o.Merchant.Name,
                    ProgramName = o.Program != null ? o.Program.Name : null,
                    AffiliateUrl = o.AffiliateUrl,
                    LandingUrl = o.LandingUrl,
                    PriceAmount = o.PriceAmount,
                    InStock = o.InStock,
                    ShippingFee = o.ShippingFee,
                    CreatedUtc = o.CreatedUtc,
                })
                .ToListAsync(cancellationToken);

            // === Meta bilgilerini oluştur ===
            var totalPages = (int)Math.Ceiling((decimal)totalCount / dto.PageSize);

            return new PagedResponse<OfferListResponseDto>
            {
                Data = items,
                Meta = new PagedResponse<OfferListResponseDto>.MetaData
                {
                    Page = dto.Page,
                    PageSize = dto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                }
            };
        }
    }
}
