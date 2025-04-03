using Microsoft.EntityFrameworkCore;

namespace OrderService.Domain.Interfaces;

public interface IUnitOfWork<TContext> where TContext : DbContext
{

    /// <summary>
    /// Gets the repository of the specified type.
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <returns></returns>
    TRepository GetRepository<TRepository>() where TRepository : class;
    
    /// <summary>
    /// Persists all changes made in the repository.
    /// </summary>
    Task SaveChangesAsync();
}