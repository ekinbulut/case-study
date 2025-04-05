using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entities;

namespace StockService.Infrastructure.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Order entity
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd();
                // Additional configuration for the Order entity can be added here
            });
            
            
            modelBuilder.Entity<Stock>().HasData(
                new Stock
                {
                    Id = Guid.Parse("a9a11000-2f6f-4a62-8d5f-d24a9eaadadf"),
                    ProductId = Guid.Parse("a9a11000-2f6f-4a62-8d5f-d24a9eaadadf"),
                    Name = "Apple Watch 44mm",
                    Quantity = 100,
                    UnitPrice = 150.75m,
                    CreatedAt = new DateTime(2025, 4, 5, 12, 0, 0, DateTimeKind.Utc)
                },
                new Stock
                {
                    Id = Guid.Parse("b1a12000-3e6f-4b62-9d6f-e25a9ebbbade"),
                    ProductId = Guid.Parse("b1a12000-3e6f-4b62-9d6f-e25a9ebbbade"),
                    Name = "Nvidia RTX 5080",
                    Quantity = 50,
                    UnitPrice = 250.50m,
                    CreatedAt = new DateTime(2025, 4, 5, 12, 0, 0, DateTimeKind.Utc)
                },
                new Stock
                {
                    Id = Guid.Parse("c2a13000-4d6f-4c62-0d7f-f36a9ecccadf"),
                    ProductId = Guid.Parse("c2a13000-4d6f-4c62-0d7f-f36a9ecccadf"),
                    Name = "Apple Iphone 14 Pro",
                    Quantity = 150,
                    UnitPrice = 85000,
                    CreatedAt = new DateTime(2025, 4, 5, 12, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}