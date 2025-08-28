using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Server.Infrastructure.Models;
using Mapster;

namespace IPodnik.Server.Infrastructure.Mappers;

public class MapperConfiguration
{
    public static void Configure()
    {
        Configure(TypeAdapterConfig.GlobalSettings);
    }

    private static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<AgentTask, AgentJobDto>()
          .Map(dest => dest.TaskType, src => src.TaskTypeId)
          .Map(dest => dest.TaskStatus, src => (TaskStatusEnum)(src.TaskStatusId ?? 0))
          .Map(dest => dest.StatusMessage, src => src.Comment)
          .Map(dest => dest.MachineName, src => src.Machine.Name);

        config.NewConfig<AgentJobDto, AgentTaskUpdateDto>()
          .Map(s => s.AgentTaskId, m => m.AgentTaskId)
          .Map(s => s.NewStatus, m => m.TaskStatus);

        config.NewConfig<AgentTaskDto, AgentTask>()
          .Map(s => s.TaskTypeId, m => (int)m.TaskType)
          .Map(s => s.TaskStatusId, m => (int)m.TaskStatus)
          .Map(s => s.MachineId, m => m.MachineId)
          .Map(s => s.UserProfileId, m => m.UserProfileId);

        config.NewConfig<AgentTask, AgentTaskDto>()
          .Map(s => s.TaskType, m => (TaskTypeEnum)m.TaskTypeId) 
          .Map(s => s.TaskStatus, m => (TaskStatusEnum)m.TaskStatusId);

        config.NewConfig<ProgressUpdateDto, AgentTaskUpdateDto>()
          .Map(dest => dest.AgentTaskId, src => src.AgentTaskId)
          .Map(dest => dest.Progress, src => src.Progress)
          .Map(dest => dest.StatusMessage, src => src.StatusMessage);

    }
}