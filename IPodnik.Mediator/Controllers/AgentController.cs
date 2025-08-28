using IPodnik.Infrastructure.DTO.MessageBus;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Mediator.Infrastructure;
using IPodnik.Mediator.Infrastructure.Operations;
using Microsoft.AspNetCore.Mvc;

namespace IPodnik.Mediator.Controllers
{
    [ApiController]
    [Route("api/task")]
    public class AgentController : ControllerBase
    {
        private readonly IRabbitQueueOperation rabbitQueueOperation;
        private readonly IServerCommunicationOperation communicationOperation;

        public AgentController(IRabbitQueueOperation rabbitQueueOperation, IServerCommunicationOperation communicationOperation)
        {
            this.rabbitQueueOperation = rabbitQueueOperation;
            this.communicationOperation = communicationOperation;
        }

        [HttpPost("PostAgentTaskStatus")]
        public async Task<IActionResult> PostAgentTaskStatus([FromBody] AgentTaskMessageDto message)
        {
            try
            {
                await rabbitQueueOperation.SendAgentTaskInfo(message);
                return StatusCode(200, true);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("GetAgentTasksPing")]
        public async Task<IActionResult> GetAgentTaskPing(HeartbeatInfoDto heartbeat)
        {
            try
            {
                var task = await communicationOperation.GetAgentTaskByMachineName(heartbeat);
                return StatusCode(200, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("GetAgentTasks")]
        public async Task<IActionResult> GetAgentTasks([FromBody] AgentTaskFilterDto filter)
        {
            try
            {
                var task = await communicationOperation.GetAgentTask(filter);
                return StatusCode(200, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
