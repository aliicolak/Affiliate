using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class ProductImage : BaseEntity
    {
        public long ProductId { get; set; }
        public long AssetId { get; set; }
        public int SortOrder { get; set; }

        public Product Product { get; set; } = null!;
        public MediaAsset Asset { get; set; } = null!;
    }
}
