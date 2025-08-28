using System;
using System.Threading.Tasks;
using MassTransit;
using IPodnik.MessageBus.Lib.DTO;

namespace IPodnik.MessageBus.Lib
{
    public interface IQueueWriter
    {
        Task PushStatus(AgentTaskMessageDto info);
        Task<bool> UpdateMessage(Guid messageId, object updatedInfo);
        Task<bool> DeleteMessage(Guid messageId);
    }

    public class QueueWriter : IQueueWriter
    {
        private readonly IBusControl busControl;

        public QueueWriter(IBusControl busControl)
        {
            this.busControl = busControl;
        }

        public async Task PushStatus(AgentTaskMessageDto info)
        {
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost:5672/taskQueue"));
            await sendEndpoint.Send(info);
        }

        public async Task<bool> UpdateMessage(Guid messageId, object updatedInfo)
        {
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost:5672/taskQueue"));
            await sendEndpoint.Send(new { messageId, updatedInfo });
            return true;
        }

        public async Task<bool> DeleteMessage(Guid messageId)
        {
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost:5672/taskQueue"));
            await sendEndpoint.Send(new { messageId, delete = true });
            return true;
        }
    }
}
