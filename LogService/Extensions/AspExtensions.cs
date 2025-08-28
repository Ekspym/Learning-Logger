using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.FileProviders;
using VueCliMiddleware;

namespace LogService.Extensions
{
    public static class AspExtensions
    {
        public static void InstallVueCli(this WebApplication app)
        {
            app.MapToVueCliProxy(
                "{*path}",
                new SpaOptions
                {
                    SourcePath = "",
                    DefaultPageStaticFileOptions = new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")),
                    }
                },
                npmScript: null,
                forceKill: true,
                wsl: false,
                runner: ScriptRunnerType.Yarn,
                regex: "READY",
                port: 8082
            );
        }
    }
}
