using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitOfWork;


public interface IUnitOfWork : IDisposable
{
    TRepository Repository<TRepository>() where TRepository : IRepository;
    Task<int> SaveChangesAsync();
}

public class UnitOfWork<T> : IUnitOfWork, IAsyncDisposable where T : DbContext
{
    private readonly T context;
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<Type, object> repositories = new ();

    protected UnitOfWork(T context, IServiceProvider serviceProvider)
    {
        this.context = context;
        this.serviceProvider = serviceProvider;
    }
    
    public TRepository Repository<TRepository>() where TRepository : IRepository
    {
        if (repositories.TryGetValue(typeof(TRepository), out var repo))
        {
            return (TRepository)repo;
        }
        
        var repository = serviceProvider.GetRequiredService<TRepository>();

        repositories.Add(typeof(TRepository), repository);
        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        foreach (var repo in repositories.Values)
        {
            var repository = repo as IDisposable;
            if(repository is not null) repository.Dispose();
        }
        
        context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var repo in repositories.Values)
        {
            var repository = repo as IAsyncDisposable;
            if (repository is not null) await repository.DisposeAsync();
        }

        
        await context.DisposeAsync();
    }
}