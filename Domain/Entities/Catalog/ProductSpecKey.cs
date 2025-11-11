using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Catalog
{
    public class ProductSpecKey : BaseEntity
    {
        public string KeyCode { get; set; } = string.Empty; // örn: cpu, ram
        public DataTypeEnum DataType { get; set; } = DataTypeEnum.Text;
        public string? Unit { get; set; }
    }
}
