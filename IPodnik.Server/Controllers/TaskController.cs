using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Server.Infrastructure.Operations;
using Microsoft.AspNetCore.Mvc;


namespace IPodnik.Server.Controllers
{
    [ApiController]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {
        private readonly Lazy<IAgentTaskOperation> agentTaskOperation;
        private readonly IAgentMonitor agentMonitor;
        private readonly ITaskQueueSynchronizer taskQueueSynchronizer;


        public TaskController(Lazy<IAgentTaskOperation> agentTaskOperation, IAgentMonitor agentMonitor, ITaskQueueSynchronizer taskQueueSynchronizer)
        {
            this.agentTaskOperation = agentTaskOperation;
            this.agentMonitor = agentMonitor;
            this.taskQueueSynchronizer = taskQueueSynchronizer;
        }

        [HttpPost("CreateAgentTask")]
        public async Task<IActionResult> CreateAgentTask([FromBody] AgentTaskOrderDto request)
        {
            try
            {
                await taskQueueSynchronizer.AddTask(request);
                return StatusCode(200);
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
                var task = await agentTaskOperation.Value.GetTasks(filter);
                return StatusCode(200, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("UpdateAgentTask")]
        public async Task<IActionResult> UpdateAgentTask([FromBody] AgentTaskUpdateDto agentTaskUpdate)
        {
            try
            {
                await taskQueueSynchronizer.UpdateTask(agentTaskUpdate);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("AgentStatus")]
        public IActionResult GetAgentStatus([FromQuery] string machineName)
        {
            var isActive = agentMonitor.IsAgentActive(machineName);
            if (isActive)
            {
                return Ok(new { Result = 1 });
            }
            else
            {
                return Ok(new { Result = 0 });
            }
        }


        [HttpPost("GetAgentTasksPing")]
        public async Task<IActionResult> GetAgentTaskByMachineName([FromBody] HeartbeatInfoDto heartbeat)
        {
            try
            {
                agentMonitor.RecordPing(heartbeat.MachineName);
                await taskQueueSynchronizer.TryUpdateTaskProgress(heartbeat);
                var task = taskQueueSynchronizer.GetTask(heartbeat.MachineName);
                return StatusCode(200, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("check")]
        public async Task<IActionResult> Check()
        {
        
            return Ok("Message sent.");
        }
    }
}
