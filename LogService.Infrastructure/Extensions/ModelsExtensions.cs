using System.Linq.Expressions;
using Infrastructure.DTO.Enums;
using LogService.Infrastructure.Contexts;

namespace LogService.Infrastructure.Extensions;

public static class ModelsExtensions
{
    public static IOrderedQueryable<AppLog> SortLogs(this IQueryable<AppLog> query, SortTypeEnum sortType, bool isAscending)
    {
        ArgumentNullException.ThrowIfNull(query);

        return sortType switch
        {
            SortTypeEnum.Id => OrderBy(query, log => log.AppLogId),
            SortTypeEnum.ApplicationName => OrderBy(query, log => log.ApplicationName),
            SortTypeEnum.VirtualMachineName => OrderBy(query, log => log.VirtualMachineName),
            SortTypeEnum.LogType => OrderBy(query, log => log.LogTypeId),
            SortTypeEnum.CreateDate => OrderBy(query, log => log.CreateDate),
            _ => throw new ArgumentException($"Unsupported sort type: {sortType}", nameof(sortType))
        };

        IOrderedQueryable<AppLog> OrderBy<TKey>(IQueryable<AppLog> source, Expression<Func<AppLog, TKey>> keySelector) => 
            source.SortLogs(keySelector, isAscending);        
    }
}