namespace Infrastructure.Core.Logging;

public interface IMachineInitializer
{
    void InitializeLogging(string loggingUrl);
}

public class BaseMachineInitializer : IMachineInitializer
{  
    public void InitializeLogging(string loggingUrl)
    {
        if (!string.IsNullOrWhiteSpace(loggingUrl))
        {
            BaseLogger.InstallBaseLogger(loggingUrl);
        }
    }
}