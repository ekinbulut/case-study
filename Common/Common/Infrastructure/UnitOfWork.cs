using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure;

/// <summary>
/// A generic Unit of Work implementation that supports retrieving repository instances.
/// </summary>
/// <typeparam name="TContext">The type of the DbContext.</typeparam>
public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    public TContext Context { get; }

    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(TContext context, IServiceProvider serviceProvider)
    {
        Context = context;
        _serviceProvider = serviceProvider;
        _repositories = new Dictionary<Type, object>();
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a repository instance from the service provider.
    /// The repository is cached so that subsequent calls return the same instance.
    /// </summary>
    /// <typeparam name="TRepository">The type of the repository interface.</typeparam>
    /// <returns>An instance of TRepository.</returns>
    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        if (_repositories.TryGetValue(typeof(TRepository), out var repository))
        {
            return repository as TRepository;
        }

        // Resolve the repository from the service provider.
        var repo = _serviceProvider.GetService(typeof(TRepository)) as TRepository;
        if (repo != null)
        {
            _repositories.Add(typeof(TRepository), repo);
        }
        else
        {
            throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} is not registered.");
        }

        return repo;
    }
}