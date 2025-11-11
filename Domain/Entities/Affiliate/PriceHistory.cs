using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Affiliate
{
    public class PriceHistory : BaseEntity
    {
        public long OfferId { get; set; }
        public decimal PriceAmount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime CapturedUtc { get; set; } = DateTime.UtcNow;

        public Offer Offer { get; set; } = null!;
    }
}
