using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class ProductTranslation : BaseEntity
    {
        public long ProductId { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = "tr";
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ShortDesc { get; set; }
        public string? LongDesc { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        public Product Product { get; set; } = null!;
    }
}
