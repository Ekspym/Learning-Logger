using EasyCaching.InMemory;
using Infrastructure.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json.Serialization;
using Infrastructure.Core.Lifestyles;

namespace Infrastructure.Core.Extensions;

public static class IocExtensions
{
    public static void InstallCache(this IServiceCollection services)
    {
        services.AddEasyCaching(options =>
        {
            options.UseInMemory(config =>
            {
                config.DBConfig = new InMemoryCachingOptions
                {
                    ExpirationScanFrequency = 60,
                    SizeLimit = null,
                    EnableReadDeepClone = false,
                    EnableWriteDeepClone = false,
                };

                config.MaxRdSecond = 120;
                config.EnableLogging = false;
                config.LockMs = 5000;
                config.SleepMs = 300;
                config.CacheNulls = true;

            }, CacheDestination.Memory.ToString());
        });

        services.AddSingleton<ICacheProvider, CacheProvider>(provider =>
        {
            var name = CacheDestination.Memory.ToString();
            var mCache = provider.GetServices<IInMemoryCaching>();
            var optionsMon = provider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<InMemoryOptions>>();
            var options = optionsMon.Get(name);

            return new CacheProvider(name, mCache, options);
        });
    }

    public static void InstallSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

        services.AddLogging().AddSerilog();
    }

    public static void InstallJsonConverters(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    }
    
    public static void InstallLazy(this IServiceCollection services)
    {
        services.AddTransient(typeof(Lazy<>));
    }
    
    public static void ScanBaseInterfaces<TAssembly>(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<TAssembly>()
                
            .AddClasses(classes => classes.AssignableTo<ISingletonHelper>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()    
                
            .AddClasses(classes => classes.AssignableTo<IScopedRepository>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            .AddClasses(classes => classes.AssignableTo<IScopedOperation>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            .AddClasses(classes => classes.AssignableTo<ITransientOperation>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
                
            .AddClasses(classes => classes.AssignableTo<ITransientFactory>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );
        
        services.AddSingleton(typeof(Factory<>));
    }
}