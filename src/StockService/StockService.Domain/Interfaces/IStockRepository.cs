using StockService.Domain.Entities;

namespace StockService.Domain.Interfaces;

public interface IStockRepository
{
    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>The order if found; otherwise, null.</returns>
    Task<Stock> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <returns>A collection of orders.</returns>
    Task<IEnumerable<Stock>> GetAllAsync();

    /// <summary>
    /// Adds a new order to the repository.
    /// </summary>
    /// <param name="order">The order to add.</param>
    Task AddAsync(Stock order);

    /// <summary>
    /// Updates an existing order in the repository.
    /// </summary>
    /// <param name="order">The order to update.</param>
    void Update(Stock order);

    /// <summary>
    /// Removes an order from the repository.
    /// </summary>
    /// <param name="order">The order to remove.</param>
    void Delete(Stock order);
}