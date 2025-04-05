using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Order entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd();
                // Additional configuration for the Order entity can be added here
            });

            var guid = Guid.Parse("01960585-7fb7-7e7d-a31c-1b2eb1681b22");
            var staticId = Guid.Parse("7b9a4f19-4f8b-4a6e-b1f1-b67af72f832d"); // Hardcoded GUID
            modelBuilder.Entity<Notification>().HasData(
                new Notification()
                {
                    Id = staticId,
                    Message = "This is a test notification",
                    CreatedAt = new DateTime(2025, 4 ,5, 12, 0, 0, DateTimeKind.Utc),
                    IsRead = false,
                    NotificationType = NotificationType.OrderCreated,
                    Status = NotificationStatus.Pending,
                    CustomerId = guid,
                }
            );
        }
    }
}