using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

namespace SportsStore.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            StoreDbContext context = app.ApplicationServices
            .CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                new Product
                {
                    Name = "Kayak",
                    Description = "A boat for one person",
                    Category = "Watersports",
                    Price = 275,
                    Image = "/images/hoalan-001.jpg"
                },
                new Product
                {
                    Name = "Lifejacket",
                    Description = "Protective and fashionagfl",
                    Category = "Watersports",
                    Price = 48.95m,
                    Image = "/images/hoalan-002.jpg"
                },
                new Product
                {
                    Name = "Soccer Ball",
                    Description = "FIFA-approved size and weight",
                    Category = "Soccer",
                    Price = 19.50m,
                    Image = "/images/hoalan-003.jpg"
                },
                new Product
                {
                    Name = "Corner Flags",
                    Description = "Give your playing field a professional touch",
                    Category = "Soccer",
                    Price = 34.95m,
                    Image = "/images/hoalan-004.jpg"
                },
                new Product
                {
                    Name = "Stadium",
                    Description = "Flat-packed 35,000-seat stadium",
                    Category = "Soccer",
                    Price = 79500,
                    Image = "/images/hoalan-005.jpg"
                },
                new Product
                {
                    Name = "Thinking Cap",
                    Description = "Improve brain efficiency by 75%",
                    Category = "Chess",
                    Price = 16,
                    Image = "/images/hoalan-006.jpg"
                },
                new Product
                {
                    Name = "Unsteady Chair",
                    Description = "Secretly give your opponent a disadvantage",
                    Category = "Chess",
                    Price = 29.95m,
                    Image = "/images/hoalan-007.jpg"
                },
                new Product
                {
                    Name = "Human Chess Board",
                    Description = "A fun game for the family",
                    Category = "Chess",
                    Price = 75,
                    Image = "/images/hoalan-008.jpg"
                },
                new Product
                {
                    Name = "Bling-Bling King",
                    Description = "Gold-plated, diamond-studded King",
                    Category = "Chess",
                    Price = 1200,
                    Image = "/images/hoalan-009.jpg"
                }
                );
                context.SaveChanges();
            }
        }
    }
}