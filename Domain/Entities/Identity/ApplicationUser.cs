using Domain.Entities.UserContent;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedUtc { get; set; }
        public bool IsBanned { get; set; } = false;

        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
