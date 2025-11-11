using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public long? LogoAssetId { get; set; }
        public bool IsActive { get; set; } = true;

        public MediaAsset? Logo { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
