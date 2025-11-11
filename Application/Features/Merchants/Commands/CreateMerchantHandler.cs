using Application.Abstractions.Persistence;
using Domain.Entities.Affiliate;
using MediatR;

namespace Application.Features.Merchants.Handlers
{
    public sealed class CreateMerchantHandler : IRequestHandler<CreateMerchantCommand, long>
    {
        private readonly IAppDbContext _context;

        public CreateMerchantHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = new Merchant
            {
                Name = request.Name,
                Slug = request.Name.ToLower().Replace(" ", "-"), // otomatik slug oluştur
                Website = request.WebsiteUrl, // eşleme burada
                IsActive = true
            };

            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync(cancellationToken);

            return merchant.Id;
        }
    }
}
