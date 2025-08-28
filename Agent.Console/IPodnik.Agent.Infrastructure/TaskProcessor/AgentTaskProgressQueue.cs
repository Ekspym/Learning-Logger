using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Infrastructure.DTO.Server;
using System.Collections.Concurrent;


namespace IPodnik.Agent.Infrastructure.TaskProcessor
{
    public interface IAgentTaskProgressQueue : ISingletonHelper
    {
        bool Add(ProgressUpdateDto agentTaskOrder);
        HeartbeatInfoDto GetInfo();
    }
    public class AgentTaskProgressQueue : IAgentTaskProgressQueue
    {
        private ConcurrentQueue<ProgressUpdateDto> agentTaskProgressQueue;

        public AgentTaskProgressQueue()
        {
            this.agentTaskProgressQueue = new ConcurrentQueue<ProgressUpdateDto>();
        }

        public bool Add(ProgressUpdateDto agentTaskOrder)
        {
            agentTaskProgressQueue.Enqueue(agentTaskOrder);
            return true;
        }

        public HeartbeatInfoDto GetInfo()
        {
            var heartbeatInfo = new HeartbeatInfoDto
            {
                MachineName = MachineInfo.Name,
                ProgressUpdates = []
            };

            while (agentTaskProgressQueue.TryDequeue(out var value))
            {
                heartbeatInfo.ProgressUpdates.Add(value);
            }

            return heartbeatInfo;
        }
    }
}
