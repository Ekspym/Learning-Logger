using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWork;

public interface IRepository
{
    // Marker interface
}

public interface IRepository<T> : IRepository where T : class
{
    Task<T> GetAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    IQueryable<T> GetQueryable();
    Task<T> GetNoTrackingAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
}

public class BaseRepository<T, C> : IDisposable, IAsyncDisposable, IRepository<T> where T : class where C : DbContext
{
    protected readonly C context;
    private readonly DbSet<T> dbSet;

    protected BaseRepository(C context)
    {
        this.context = context;
        dbSet = this.context.Set<T>();
    }

    public async Task<T> GetAsync(int id)
    {
        return await dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public IQueryable<T> GetQueryable()
    {
        return dbSet.AsQueryable();
    }

    public void Dispose()
    {
        if (context is IDisposable contextDisposable)
            contextDisposable.Dispose();
        else
            _ = context.DisposeAsync().AsTask();
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
    }

    public async Task<T> GetNoTrackingAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbSet.AsNoTracking().SingleOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await context.Set<T>().AddRangeAsync(entities);
        await context.SaveChangesAsync();
        return entities;
    }

}