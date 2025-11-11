using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Affiliate
{
    public class Offer : BaseEntity
    {
        public long ProductId { get; set; }
        public long MerchantId { get; set; }
        public long? ProgramId { get; set; }
        public string AffiliateUrl { get; set; } = string.Empty;
        public string? LandingUrl { get; set; }
        public decimal PriceAmount { get; set; }
        public int CurrencyId { get; set; }
        public bool InStock { get; set; } = true;
        public decimal? ShippingFee { get; set; }

        public Merchant Merchant { get; set; } = null!;
        public AffiliateProgram? Program { get; set; }
        public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    }
}
