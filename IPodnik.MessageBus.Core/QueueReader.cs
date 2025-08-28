using Infrastructure.DTO.MessageBus;
using MassTransit;

public interface IQueueReader
{
    Task<AgentTaskMessageDto?> GetLatestMessageAsync();
}

public class QueueReader : IQueueReader, IConsumer<AgentTaskMessageDto>
{
    private TaskCompletionSource<AgentTaskMessageDto?>? tcs;

    public async Task<AgentTaskMessageDto?> GetLatestMessageAsync()
    {
        tcs = new TaskCompletionSource<AgentTaskMessageDto?>(TaskCreationOptions.RunContinuationsAsynchronously);
        return await tcs.Task;
    }

    public Task Consume(ConsumeContext<AgentTaskMessageDto> context)
    {
        tcs?.TrySetResult(context.Message);
        return Task.CompletedTask;
    }
}
