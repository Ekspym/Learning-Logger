using System.Text.Json;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Lifestyles;
using Infrastructure.DTO.Enums;
using Infrastructure.DTO.LogService;
using LogService.Infrastructure.Helpers;
using LogService.Infrastructure.Repository;
using Mapster;
using Serilog;

namespace LogService.Infrastructure.Operations
{
    public interface ILoggerOperation : ITransientFactory
    {
        /// <summary>
        /// Logs a general message with details.
        /// </summary>
        Task LogGeneralAsync(string message, string appName, string virtualMachine, DateTime createDate, LogTypeEnum type);

        /// <summary>
        /// Logs a bulk list of application logs.
        /// </summary>
        /// <param name="appLogs">The list of logs to add.</param>
        Task LogBulkAsync(List<AppLogDto> appLogs);
        
        /// <summary>
        /// Inserts Logs into database, resolves cache of virtual machine names.
        /// </summary>
        Task<bool> ProcessAppLogs(List<AppLogDto> appLogs);

        /// <summary>
        /// Retrieves logs based on a specified filter.
        /// </summary>
        Task<List<LogInfoDto>> GetAppLogsByFilter(LogFilterDto logFilter);

        /// <summary>
        /// Retrieves all virtual machine names using a cache.
        /// </summary>
        Task<HashSet<string>> GetAllVirtualMachineNamesCachedAsync();

        /// <summary>
        /// Check for names in cache and marks it for refresh if names are missing.
        /// </summary>
        Task HandleMachineNamesCacheRefresh(List<AppLogDto> machineNames);
        /// <summary>
        /// Retrieves a log by its ID.
        /// </summary>
        Task<LogInfoDto> GetAppLogById(int id);
        
        /// <summary>
        ///  Enqueues a log to be processed.
        /// </summary>
        bool EnqueueAppLog(AppLogDto request);
    }
    public class LoggerOperation : BaseCachingOperation, ILoggerOperation
    {
        private readonly ILogRepository logRepository;
        private readonly IAppLogQueueProvider appLogQueueProvider;

        public LoggerOperation(ILogRepository logRepository, 
            IAppLogQueueProvider appLogQueueProvider)
        {
            this.logRepository = logRepository;
            this.appLogQueueProvider = appLogQueueProvider;
        }

        public async Task<bool> ProcessAppLogs(List<AppLogDto> appLogs)
        {
            try
            {
                await LogBulkAsync(appLogs);
                await HandleMachineNamesCacheRefresh(appLogs);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while consuming logs. Logs: {@logs}", JsonSerializer.Serialize(appLogs));
                return false;
            }
        }

        public async Task<List<LogInfoDto>> GetAppLogsByFilter(LogFilterDto logFilter)
        {
            var logs = await logRepository.GetByFilter(logFilter);
            return logs.Adapt<List<LogInfoDto>>();
        }

        public Task LogGeneralAsync(string message, string appName, string virtualMachine, DateTime createDate, LogTypeEnum type)
        {
            return logRepository.AddAsync(message, appName, virtualMachine, createDate, type);
        }

        public Task LogBulkAsync(List<AppLogDto> appLogs)
        {
            return logRepository.AddBulkAsync(appLogs);
        }

        public async Task<HashSet<string>> GetAllVirtualMachineNames()
        {
            var machines = await logRepository.GetAllVirtualMachineNames();
            return machines.ToHashSet();
        }

        public Task<HashSet<string>> GetAllVirtualMachineNamesCachedAsync()
        {
            return ExecuteMemoryCachedAsync(GetAllVirtualMachineNames, Cache.GetKey(nameof(GetAllVirtualMachineNames)));
        }

        public async Task HandleMachineNamesCacheRefresh(List<AppLogDto> machineNames)
        {
            var cached = await GetAllVirtualMachineNamesCachedAsync();
            var refreshCache = machineNames.Any(x => !cached.Contains(x.VirtualMachineName));

            if (refreshCache)
            {
                RemoveFromCache([Cache.GetKey(nameof(GetAllVirtualMachineNames))]);
            }
        }

        public async Task<LogInfoDto> GetAppLogById(int id)
        {
            var log = await logRepository.GetById(id);
            return log.Adapt<LogInfoDto>();
        }

        public bool EnqueueAppLog(AppLogDto request)
        {
            var success = appLogQueueProvider.TryEnqueue(request);

            if (!success)
            {
                Log.Error("Error occured during enqueuing log. Request: {@request}", JsonSerializer.Serialize(request));
            }

            return success;
        }
    }
}
