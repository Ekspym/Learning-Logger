using Infrastructure.DTO.MessageBus;

namespace MessageBus.Core
{
    public interface IMessageHandler
    {
        Task HandleMessage(AgentTaskMessageDto message);
    }

    public class MessageHandler : IMessageHandler
    {
        public async Task HandleMessage(AgentTaskMessageDto message)
        {
            Console.WriteLine($"Received message with TaskId: {message.TaskType}");
            await Task.CompletedTask;
        }
    }

}
