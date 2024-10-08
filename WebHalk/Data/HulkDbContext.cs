﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebHalk.Data.Entities;

namespace WebHalk.Data
{
    public class HulkDbContext : IdentityDbContext<IdentityUser>
    {
        public HulkDbContext(DbContextOptions<HulkDbContext> options) : base(options) { }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
    }
}
