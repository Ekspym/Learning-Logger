using EasyCaching.Core;
using EasyCaching.InMemory;

namespace Infrastructure.Core.Caching;

public interface ICacheProvider : IEasyCachingProvider
{
    void RemoveFromCache(IEnumerable<string> cacheKeys);
}

public class CacheProvider : DefaultInMemoryCachingProvider, ICacheProvider
{
    public CacheProvider(string name, IEnumerable<IInMemoryCaching> cache, InMemoryOptions options) : base(name, cache, options, null)
    {
        
    }

    public void RemoveFromCache(IEnumerable<string> cacheKeys)
    {
        RemoveAll(cacheKeys);
    }
}
