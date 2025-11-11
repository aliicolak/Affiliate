using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class Product : BaseEntity
    {
        public string Slug { get; set; } = string.Empty;
        public string? Sku { get; set; }
        public long? BrandId { get; set; }
        public long? DefaultCategoryId { get; set; }
        public long? PrimaryImageId { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public decimal RatingAvg { get; set; }
        public int RatingCount { get; set; }
        public bool IsActive { get; set; } = true;

        public Brand? Brand { get; set; }
        public Category? DefaultCategory { get; set; }
        public MediaAsset? PrimaryImage { get; set; }

        public ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
        public ICollection<ProductSpecValue> SpecValues { get; set; } = new List<ProductSpecValue>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
