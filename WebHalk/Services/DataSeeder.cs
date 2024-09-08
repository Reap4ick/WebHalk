﻿using WebHalk.Data.Entities;
using WebHalk.Data;
using Microsoft.AspNetCore.Identity;
using WebHalk.Data.Entities.Identity;
using WebHalk.Constants;

namespace WebHalk.Services
{
    public class DataSeeder
    {
        private readonly HulkDbContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;

        public DataSeeder(HulkDbContext context, UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
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

                for (int i = 1; i <= 100; i++)
                {
                    var product = new ProductEntity
                    {
                        Name = $"Ноутбук {i}",  // Назва буде генеруватись автоматично
                        Category = c1,
                        Price = 2000 + i * 10  // Генерування різних цін для кожного продукту
                    };

                    _context.Products.Add(product);

                    // Додавання кількох зображень для кожного продукту
                    _context.ProductImages.AddRange(
                        new ProductImageEntity { Image = $"p_{i}(1).webp", Product = product },
                        new ProductImageEntity { Image = $"p_{i}(2).webp", Product = product },
                        new ProductImageEntity { Image = $"p_{i}(3).webp", Product = product }
                    );
                }

                _context.SaveChanges();
            }
        }


        public void SeedRolesAndUsers()
        {
            // seed roles
            if (_context.Roles.Count() == 0)
            {
                var roles = new[]
                {
                    new RoleEntity { Name = Roles.Admin },
                    new RoleEntity { Name = Roles.User }
                };

                foreach (var role in roles)
                {
                    var outcome = _roleManager.CreateAsync(role).Result;
                    if (!outcome.Succeeded) Console.WriteLine($"Failed to create role: {role.Name}");
                }
            }

            // seed users
            if (_context.Users.Count() == 0)
            {
                var users = new[]
                {
                    new { User = new UserEntity { FirstName = "Tony", LastName = "Stark", UserName = "admin@gmail.com", Email = "admin@gmail.com" }, Password = "admin1", Role = Roles.Admin },
                    new { User = new UserEntity { FirstName = "Boba", LastName = "Gray", UserName = "user@gmail.com", Email = "user@gmail.com" }, Password = "bobapass1", Role = Roles.User },
                    new { User = new UserEntity { FirstName = "Biba", LastName = "Undefined", UserName = "biba@gmail.com", Email = "biba@gmail.com" }, Password = "bibapass3", Role = Roles.User }
                };

                foreach (var i in users)
                {
                    var outcome = _userManager.CreateAsync(i.User, i.Password).Result;

                    if (!outcome.Succeeded)
                        Console.WriteLine($"Failed to create user: {i.User.UserName}");
                    else
                        outcome = _userManager.AddToRoleAsync(i.User, i.Role).Result;
                }
            }
        }
    }
}