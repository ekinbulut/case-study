using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>The order if found; otherwise, null.</returns>
    Task<Notification> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <returns>A collection of orders.</returns>
    Task<IEnumerable<Notification>> GetAllAsync();

    /// <summary>
    /// Adds a new order to the repository.
    /// </summary>
    /// <param name="order">The order to add.</param>
    Task AddAsync(Notification order);

    /// <summary>
    /// Updates an existing order in the repository.
    /// </summary>
    /// <param name="order">The order to update.</param>
    void Update(Notification order);

    /// <summary>
    /// Removes an order from the repository.
    /// </summary>
    /// <param name="order">The order to remove.</param>
    void Delete(Notification order);
}