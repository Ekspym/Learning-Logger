using Infrastructure.Core.Caching;
using Infrastructure.Core.Extensions;
using LogService.Extensions;
using LogService.Infrastructure.Extensions;
using LogService.Infrastructure.Mappers;
using LogService.Infrastructure.Services;

namespace LogService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            MapperConfiguration.Configure();
            // Add services to the container.
            IServiceCollection services = builder.Services;

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.InstallLoggerService();
            services.ScanBaseInterfaces<LoggerService>();
            
            services.InstallLogContext(builder.Configuration);
            services.InstallCache();
            services.InstallSerilog(builder.Configuration);
            services.InstallJsonConverters();
   
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseCors();
            app.UseRouting();

            Cache.Initialize(app.Services.GetService<ICacheProvider>());

            #if RELEASE
            app.InstallVueCli();
            #endif

            app.MapControllers();

            app.Run();
        }
    }
}
