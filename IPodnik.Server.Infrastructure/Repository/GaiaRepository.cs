using IPodnik.Infrastructure.UnitOfWork;
using IPodnik.Server.Infrastructure.Models;

namespace IPodnik.Server.Infrastructure.Repository;

public class GaiaRepository<T> : BaseRepository<T, IPodnikGaiaContext> where T : class
{
    public GaiaRepository(IPodnikGaiaContext context) : base(context)
    {
    }
}

public class GaiaUnitOfWork : UnitOfWork<IPodnikGaiaContext>
{
    public GaiaUnitOfWork(IPodnikGaiaContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
    }
}

public interface IGaiaRepository<T> : IRepository<T> where T : class
{
}
