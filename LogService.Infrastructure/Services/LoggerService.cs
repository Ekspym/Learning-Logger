using Infrastructure.Core.Lifestyles;
using LogService.Infrastructure.Helpers;
using LogService.Infrastructure.Operations;
using Microsoft.Extensions.Hosting;

namespace LogService.Infrastructure.Services;

public class LoggerService : IHostedService
{
    private readonly IAppLogQueueProvider appLogQueueProvider;
    private readonly Factory<ILoggerOperation> loggerOperationFactory;
    
    private Task loop;
    
    public LoggerService(IAppLogQueueProvider appLogQueueProvider, Factory<ILoggerOperation> loggerOperationFactory) 
    {
        this.appLogQueueProvider = appLogQueueProvider;
        this.loggerOperationFactory = loggerOperationFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        loop = Task.Run(() => ConsumingLoopAsync(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await loop;
    }
    
    private async Task ConsumingLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var logs = appLogQueueProvider.DequeueBatch(cancellationToken);

            if (logs.Count > 0)
            {
                var loggerOperation = loggerOperationFactory.Create();
                
                var success = await loggerOperation.ProcessAppLogs(logs);
                if (!success)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }
    }
}

