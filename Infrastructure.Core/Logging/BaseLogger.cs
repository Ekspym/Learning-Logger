using Flurl;
using Flurl.Http;
using Infrastructure.DTO.Enums;
using Infrastructure.DTO.LogService;
using System.Collections.Concurrent;

namespace Infrastructure.Core.Logging
{
    public static class BaseLogger
    {
        private static readonly ConcurrentQueue<AppLogDto> logQueue = new ();
        private static bool isRunning;
        private static string machineName;
        private static string appName;
        
        public static void InstallBaseLogger(string url)
        {
            machineName = MachineInfo.Name;
            appName = MachineInfo.ApplicationName;
            Init(url);
        }

        private static void Init(string baseUrl)
        {
            if (isRunning) return;
            isRunning = true;

            Task.Run(async () =>
            {
                while (true)
                {
                    if (logQueue.TryDequeue(out AppLogDto log))
                    {
                        await SendLogAsync(baseUrl, log);
                    }
                    await Task.Delay(100);
                }
            });
        }

        public static void Log(AppLogDto log)
        {
            logQueue.Enqueue(log);
        }

        public static void LogError(string title, string virtualMachineName, string message, string applicationName, DateTime createDate)
        {
            Log(new AppLogDto()
            {
                Title = title,
                VirtualMachineName = virtualMachineName,
                LogType = LogTypeEnum.Error,
                Message = message,
                ApplicationName = applicationName,
                CreateDate = createDate
            });
        }

        public static void LogError(string title, string message, DateTime createDate)
        {
            Log(new AppLogDto()
            {
                Title = title,
                VirtualMachineName = machineName,
                LogType = LogTypeEnum.Error,
                Message = message,
                ApplicationName = appName,
                CreateDate = createDate
            });
        }

        public static void LogInformation(string title, string virtualMachineName, string message, string applicationName, DateTime createDate)
        {
            Log(new AppLogDto
            {
                Title = title,
                VirtualMachineName = virtualMachineName,
                LogType = LogTypeEnum.Information,
                Message = message,
                ApplicationName = applicationName,
                CreateDate = createDate
            });
        }

        public static void LogInformation(string title, string message, DateTime createDate)
        {
            Log(new AppLogDto
            {
                Title = title,
                VirtualMachineName = machineName,
                LogType = LogTypeEnum.Information,
                Message = message,
                ApplicationName = appName,
                CreateDate = createDate
            });
        }

        private static async Task SendLogAsync(string baseUrl, AppLogDto log)
        {
            try
            {
                await baseUrl
                         .AppendPathSegment("api/log/InsertToLog")
                         .PutJsonAsync(log);
            }
            catch (FlurlHttpException ex)
            {
                var errorDetails = await ex.GetResponseStringAsync();
                Serilog.Log.Error(ex, $"Failed to send log {errorDetails}");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "");
            }
        }

    }
}
