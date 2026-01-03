using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class CategoryTranslation : BaseEntity
    {
        public long CategoryId { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = "tr";
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        public Category Category { get; set; } = null!;
    }
}
