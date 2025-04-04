using Common.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Commands;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;

namespace StockService.Application.Handlers;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, bool>
{
    private readonly IUnitOfWork<StockDbContext> _unitOfWork;

    public UpdateStockCommandHandler(IUnitOfWork<StockDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.GetRepository<IStockRepository>();
        var stock = await repository.GetByIdAsync(request.ProductId);
        if (stock!=null)
        {
            try
            {
                stock.Quantity = request.Quantity;
                stock.UpdatedAt = DateTime.UtcNow;
                repository.Update(stock);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Handle the error (e.g., log it) and throw a new exception or return an error result.
                throw new Exception("An error occurred while saving the order. Please try again later.", ex);
            }
        }
        return false;
    }
    
}