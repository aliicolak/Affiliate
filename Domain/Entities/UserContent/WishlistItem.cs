using Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserContent
{
    public class WishlistItem
    {
        public long WishlistId { get; set; }
        public long ProductId { get; set; }
        public string? Note { get; set; }

        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
