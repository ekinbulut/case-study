using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entities;

namespace StockService.Infrastructure.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {
        }

        public DbSet<Stock> Orders { get; set; }

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
            
            modelBuilder.Entity<StockItem>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd();
                // Additional configuration for the Order Item entity can be added here
            });
        }
    }
}