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
        return await _context.Stocks.FirstOrDefaultAsync(o => o.ProductId == id);
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _context.Stocks.ToListAsync();
    }

    public async Task AddAsync(Stock order)
    {
        await _context.Stocks.AddAsync(order);
    }

    public void Update(Stock order)
    {
        _context.Stocks.Update(order);
    }

    public void Delete(Stock order)
    {
        _context.Stocks.Remove(order);
    }
}