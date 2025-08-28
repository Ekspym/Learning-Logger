using MassTransit;
using IPodnik.Server.Infrastructure.Operations;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.MessageBus;
using Mapster;

public class AgentTaskMessageHandler : IConsumer<AgentTaskMessageDto>
{
    private readonly ITaskQueueSynchronizer taskQueueSynchronizer;
    public AgentTaskMessageHandler(ITaskQueueSynchronizer taskQueueSynchronizer)
    {
        this.taskQueueSynchronizer = taskQueueSynchronizer;
    }

    public async Task Consume(ConsumeContext<AgentTaskMessageDto> context)
    {
        await HandleMessage(context.Message);
    }

    private async Task HandleMessage(AgentTaskMessageDto message)
    { 
        await taskQueueSynchronizer.TryUpdateTask(new AgentTaskUpdateDto()
        {
            AgentTaskId = message.AgentTaskId,
            NewStatus = message.Status,
            Updated = message.Updated,
            Progress = message.Progress,
            StatusMessage = message.StatusMessage
        });
    }
}
