using Serilog;

namespace Infrastructure.Core.Caching;

public abstract class BaseCachingOperation
{
    protected async Task<TResult> ExecuteMemoryCachedAsync<TResult>(
        Func<Task<TResult>> function,
        string cacheKey,
        bool ignoreCachedValue = false,
        bool cacheNullValue = false)
    {
        return await ExecuteMemoryCachedAsync(function, cacheKey, Constants.DefaultTimeToLive, ignoreCachedValue, cacheNullValue);
    }

    protected async Task<TResult> ExecuteMemoryCachedAsync<TResult>(
        Func<Task<TResult>> function,
        string cacheKey,
        TimeSpan expiryPreset,
        bool ignoreCachedValue = false,
        bool cacheNullValue = false)
    {
        
        var cacheProvider = Cache.Provider;
        
        if (cacheProvider == null)
        { 
            return await function();
        }

        var cached = cacheProvider.Get<TResult>(cacheKey);

        if (cached.HasValue && !ignoreCachedValue)
        {
            return cached.Value;
        }
        
        try
        {
            var result = await function.Invoke();

            if (result != null || cacheNullValue)
            {
                cacheProvider.Set(cacheKey, result, expiryPreset);
            }

            return result;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "");
        }

        return cached.Value;
    }

    protected void RemoveFromCache(string[] cacheKey)
    {
        Cache.Provider.RemoveFromCache(cacheKey);
    }
}
