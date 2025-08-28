using MassTransit;
using Infrastructure.DTO.MessageBus;


namespace MessageBus.Core
{
    public interface IQueueWriter
    {
        Task PushStatus(AgentTaskMessageDto info);
    }

    public class QueueWriter : IQueueWriter
    {
        private readonly ISendEndpointProvider sendEndpointProvider;

        public QueueWriter(ISendEndpointProvider sendEndpointProvider)
        {
            this.sendEndpointProvider = sendEndpointProvider;
        }

        public async Task PushStatus(AgentTaskMessageDto info)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:taskQueue"));
            await sendEndpoint.Send(info);
        }
    }
}
