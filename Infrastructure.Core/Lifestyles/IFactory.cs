using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Core.Lifestyles;

public interface ITransientFactory
{
    
}

public class Factory<T> 
{
    private readonly IServiceProvider _serviceProvider;

    public Factory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create()
    {
        return _serviceProvider.GetRequiredService<IServiceScopeFactory>()
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<T>();
    }
}
