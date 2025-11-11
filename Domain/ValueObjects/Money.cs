using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public record Money(decimal Amount, string Currency)
    {
        public override string ToString() => $"{Amount:0.00} {Currency}";
    }
}
