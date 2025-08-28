using System.Collections.Concurrent;
using Infrastructure.Core.Extensions;
using Infrastructure.Core.Lifestyles;
using Infrastructure.DTO.LogService;

namespace LogService.Infrastructure.Helpers;

public interface IAppLogQueueProvider : ISingletonHelper
{
    bool TryEnqueue(AppLogDto log);
    List<AppLogDto> DequeueBatch(CancellationToken cancellationToken);
}

public class AppLogQueueProvider : IAppLogQueueProvider
{
    private readonly BlockingCollection<AppLogDto> appLogEntries;

    public AppLogQueueProvider()
    {
        appLogEntries = new BlockingCollection<AppLogDto>();
    }

    public bool TryEnqueue(AppLogDto log)
    {
        return appLogEntries.TryAdd(log);
    }

    public List<AppLogDto> DequeueBatch(CancellationToken cancellationToken)
    {
        var logs = new List<AppLogDto>();
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.LogDequeueWaitSeconds));
        
        while (!cancellationToken.IsCancellationRequested && logs.Count < Constants.LogDequeueCount && appLogEntries.TryTakeAndWait(out var logEntry, cancellationTokenSource.Token))
        {
            logs.Add(logEntry);
        }

        return logs;
    }
}

