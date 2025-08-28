using Infrastructure.Core.Extensions;
using Infrastructure.Core.Lifestyles;
using Infrastructure.DTO.Enums;
using Infrastructure.DTO.LogService;
using LogService.Infrastructure.Contexts;
using LogService.Infrastructure.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LogService.Infrastructure.Repository;

public interface ILogRepository : IScopedRepository
{
    Task AddAsync(AppLog log);
    Task AddAsync(string message, string appName, string virtualMachine, DateTime createDate, LogTypeEnum type);
    Task<List<AppLog>> GetByFilter(LogFilterDto logFilter);
    Task AddBulkAsync(List<AppLogDto> appLogs);
    Task<List<string>> GetAllVirtualMachineNames();
    Task<AppLog> GetById(int id);
}

public class LogRepository : ILogRepository
{
    private LoggerContext dbContext;

    public LogRepository(LoggerContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddBulkAsync(List<AppLogDto> appLogs)
    {
        try
        {
            dbContext.AddRange(appLogs.Adapt<List<AppLog>>());
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during {@addBulkAsync}", nameof(AddBulkAsync));
        }
    }
    
    public async Task AddAsync(AppLog log)
    {
        try
        {
            dbContext.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during {@addAsync}", nameof(AddAsync));
        }
    }

    public Task AddAsync(string message, string appName, string virtualMachine, DateTime createDate, LogTypeEnum type)
    {
        var log = new AppLog()
        {
            LogTypeId = (int)type,
            Message = message,
            ApplicationName = appName,
            VirtualMachineName = virtualMachine,
            CreateDate = createDate
        };
        
        return AddAsync(log);
    }

    public async Task<List<AppLog>> GetByFilter(LogFilterDto logFilter)
    {
        if (logFilter.LogCount > 500)
        {
            logFilter.LogCount = 500;
        }

        IQueryable<AppLog> matches = dbContext.AppLogs.AsQueryable();

        if (logFilter.FromId.HasValue)
        {
            matches = matches.Where(l => l.AppLogId >= logFilter.FromId);
        }

        if (logFilter.ToId.HasValue)
        {
            matches = matches.Where(l => l.AppLogId <= logFilter.ToId);
        }

        if (logFilter.LogType != null)
        {
            var filterTypes = logFilter.LogType.SelectList(t => (int)t);
            matches = matches.Where(l => filterTypes.Contains(l.LogTypeId));
        }

        if (logFilter.ApplicationName != null)
        {
            matches = matches.Where(l => logFilter.ApplicationName.Contains(l.ApplicationName));
        }

        if (logFilter.VirtualMachineName != null)
        {
            matches = matches.Where(l => logFilter.VirtualMachineName.Contains(l.VirtualMachineName));
        }

        if (logFilter.FromDate.HasValue)
        {
            matches = matches.Where(l => l.CreateDate >= logFilter.FromDate);
        }

        if (logFilter.ToDate.HasValue)
        {
            matches = matches.Where(l => l.CreateDate < logFilter.ToDate);
        }
        
        matches = matches.SortLogs(logFilter.SortType, logFilter.IsAscending);

        return await matches
            .Skip(logFilter.Page * logFilter.LogCount)
            .Take(logFilter.LogCount)
            .ToListAsync();
    }

    public Task<List<string>> GetAllVirtualMachineNames()
    {
        return dbContext.AppLogs
            .GroupBy(al => al.VirtualMachineName)
            .Select(al => al.Key)
            .ToListAsync();
    }

    public async Task<AppLog> GetById(int id)
    {
        return await dbContext.AppLogs
            .Where(m => m.AppLogId == id)
            .FirstOrDefaultAsync();
    }
}
