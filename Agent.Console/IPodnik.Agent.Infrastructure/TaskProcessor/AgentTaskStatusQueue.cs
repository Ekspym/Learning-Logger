using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.MessageBus;
using System.Collections.Concurrent;


namespace IPodnik.Agent.Infrastructure.TaskProcessor
{
    public interface IAgentTaskStatusQueue :ISingletonHelper
    {
        bool Add(AgentTaskMessageDto agentTaskOrder);
        AgentTaskMessageDto Dequeue();
    }
    public class AgentTaskStatusQueue : IAgentTaskStatusQueue
    {
        private ConcurrentQueue<AgentTaskMessageDto> agentTaskStatusQueue;

        public AgentTaskStatusQueue()
        {
            this.agentTaskStatusQueue = new ConcurrentQueue<AgentTaskMessageDto>();
        }

        public bool Add(AgentTaskMessageDto agentTaskOrder)
        {
            agentTaskStatusQueue.Enqueue(agentTaskOrder);
            return true;
        }

        public AgentTaskMessageDto Dequeue()
        {
            return agentTaskStatusQueue.TryDequeue(out var newest) ? newest : null;
        }
    }
}
