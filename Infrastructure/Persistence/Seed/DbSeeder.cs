using Domain.Entities.Affiliate;
using Domain.Entities.Catalog;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // DB yoksa oluştur
            await context.Database.MigrateAsync();

            // === ROLLER ===
            string[] roles = { "Admin", "User" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.Roles.AnyAsync(r => r.Name == roleName))
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        CreatedUtc = DateTime.UtcNow
                    };
                    await roleManager.CreateAsync(role);
                }
            }

            // === ADMIN USER ===
            var adminEmail = "admin@affiliate.tech";
            var adminUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    DisplayName = "System Admin",
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    CreatedUtc = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123*");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // === ÖRNEK MERCHANT ===
            if (!await context.Merchants.AnyAsync())
            {
                var merchant = new Merchant
                {
                    Name = "TechZone",
                    Slug = "techzone",
                    Website = "https://techzone.example.com",
                    IsActive = true,
                    CreatedUtc = DateTime.UtcNow
                };
                context.Merchants.Add(merchant);
                await context.SaveChangesAsync();

                var program = new AffiliateProgram
                {
                    MerchantId = merchant.Id,
                    BaseCommissionPct = 8.5m,
                    CookieDays = 30,
                    DefaultCurrencyId = 1,
                    TrackingDomain = "track.techzone.com",
                    CreatedUtc = DateTime.UtcNow
                };
                context.AffiliatePrograms.Add(program);
                await context.SaveChangesAsync();
            }

            // === ÖRNEK BRAND / CATEGORY ===
            if (!await context.Brands.AnyAsync())
            {
                context.Brands.Add(new Brand
                {
                    Name = "HyperTech",
                    Slug = "hypertech",
                    IsActive = true,
                    CreatedUtc = DateTime.UtcNow
                });
            }

            if (!await context.Categories.AnyAsync())
            {
                context.Categories.Add(new Category
                {
                    Slug = "electronics",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedUtc = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
