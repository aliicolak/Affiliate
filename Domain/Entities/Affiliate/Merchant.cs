using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Affiliate
{
    public class Merchant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Website { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }
}
