using Infrastructure.DTO.Enums;
using Infrastructure.DTO.LogService;
using Mapster;
using LogService.Infrastructure.Contexts;

namespace LogService.Infrastructure.Mappers;


public static class MapperConfiguration
{
    public static void Configure()
    {
        Configure(TypeAdapterConfig.GlobalSettings);
    }

    private static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<AppLog, LogInfoDto>()
            .Map(s => s.LogType, m => (LogTypeEnum)m.LogTypeId)
            .Map(s => s.Id, m => m.AppLogId);
        
        config.NewConfig<AppLogDto, AppLog>()
            .Map(s => s.LogTypeId, m => (int)m.LogType);
    }
}
