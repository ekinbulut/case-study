using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }
        
    public async Task<Notification> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications.ToListAsync();
    }

    public async Task AddAsync(Notification order)
    {
        await _context.Notifications.AddAsync(order);
    }

    public void Update(Notification order)
    {
        _context.Notifications.Update(order);
    }

    public void Delete(Notification order)
    {
        _context.Notifications.Remove(order);
    }
}