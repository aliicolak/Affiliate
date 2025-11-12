using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Affiliate
{
    public class AffiliateProgram : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // ✅ EKLENDİ

        public long MerchantId { get; set; }
        public int DefaultCurrencyId { get; set; }
        public decimal BaseCommissionPct { get; set; }
        public int CookieDays { get; set; } = 7;
        public string? TrackingDomain { get; set; }

        public Merchant Merchant { get; set; } = null!;
    }
}
