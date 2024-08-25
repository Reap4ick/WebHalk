using Microsoft.AspNetCore.Identity;
using WebHalk.Data.Entities;
using WebHalk.Data;

namespace WebHalk.Services
{
    public class DataSeeder
    {
        private readonly HulkDbContext _context;

        public DataSeeder(HulkDbContext context)
        {
            _context = context;
        }

        // Seed продуктів
        public void SeedProducts()
        {
            if (_context.Products.Count() == 0)
            {
                var c1 = new CategoryEntity
                {
                    Name = "Laptops",
                    Image = "eb47fa37-007c-4d3e-be5e-a5ccb7600320.jpg"
                };

                _context.Categories.Add(c1);
                _context.SaveChanges();

                var p1 = new ProductEntity
                {
                    Name = "Ноутбук HP EliteBook 840 G10",
                    Category = c1,
                    Price = 2350.00m
                };

                var p2 = new ProductEntity
                {
                    Name = "Ноутбук Dell Latitude 7640",
                    Category = c1,
                    Price = 2020.00m
                };

                _context.Products.AddRange(p1, p2);
                _context.ProductImages.AddRange(
                    new ProductImageEntity { Image = "p_1(1).webp", Product = p1 },
                    new ProductImageEntity { Image = "p_1(2).webp", Product = p1 },
                    new ProductImageEntity { Image = "p_1(3).webp", Product = p1 },

                    new ProductImageEntity { Image = "p_2(1).webp", Product = p2 },
                    new ProductImageEntity { Image = "p_2(2).webp", Product = p2 },
                    new ProductImageEntity { Image = "p_2(3).webp", Product = p2 }
                );
                _context.SaveChanges();
            }
        }

        // Seed користувачів і ролей
        public async Task SeedUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (userManager.Users.All(u => u.UserName != "admin@example.com"))
            {
                var user = new IdentityUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com"
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
