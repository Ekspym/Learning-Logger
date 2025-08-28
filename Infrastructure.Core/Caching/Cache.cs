namespace Infrastructure.Core.Caching;

public static class Cache
{
    private const string DefaultPrefix = "Default";
    public static ICacheProvider Provider { get; set; }
    
    public static void Initialize(ICacheProvider cacheProvider)
    {
        Provider = cacheProvider;
    }
    
    public static string GetKey(Type type, params object[] parameters)
        => GetKey(type.FullName, parameters);

    public static string GetKey(string typeIdentificator, params object[] parameters)
    {
        bool hasParam = parameters.Length > 0;

        return $"{DefaultPrefix}.{typeIdentificator}" + (hasParam ? $"({string.Join(",", parameters)})" : "");
    }
}