using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.MessageBus;
using IPodnik.MessageBus.Core;

namespace IPodnik.Mediator.Infrastructure
{
    public interface IRabbitQueueOperation : IScopedOperation
    {
        Task SendAgentTaskInfo(AgentTaskMessageDto message);
    }

    public class RabbitQueueOperation : IRabbitQueueOperation
    {
        private readonly IQueueWriter queueWriter;

        public RabbitQueueOperation(IQueueWriter queueWriter)
        {
            this.queueWriter = queueWriter;
        }

        public async Task SendAgentTaskInfo(AgentTaskMessageDto message)
        {
            await queueWriter.PushStatus(message);
        }
    }
}
