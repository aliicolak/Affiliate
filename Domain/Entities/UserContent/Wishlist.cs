using Domain.Common;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserContent
{
    public class Wishlist : BaseEntity
    {
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
