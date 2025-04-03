using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entities;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;

namespace StockService.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly StockDbContext _context;

    public StockRepository(StockDbContext context)
    {
        _context = context;
    }
        
    public async Task<Stock> GetByIdAsync(Guid id)
    {
        return await _context.Orders.Include(o=> o.Items).FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task AddAsync(Stock order)
    {
        await _context.Orders.AddAsync(order);
    }

    public void Update(Stock order)
    {
        _context.Orders.Update(order);
    }

    public void Delete(Stock order)
    {
        _context.Orders.Remove(order);
    }
}