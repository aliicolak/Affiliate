using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class ProductSpecValue
    {
        public long ProductId { get; set; }
        public long SpecKeyId { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNum { get; set; }
        public bool? ValueBool { get; set; }

        public Product Product { get; set; } = null!;
        public ProductSpecKey SpecKey { get; set; } = null!;
    }
}
