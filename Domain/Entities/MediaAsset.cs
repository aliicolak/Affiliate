using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MediaAsset : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? ContentType { get; set; }
    }
}
