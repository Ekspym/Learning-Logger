using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Infrastructure.UnitOfWork;
using IPodnik.Server.Infrastructure.Models;
using IPodnik.Server.Infrastructure.Repository;
using Mapster;

namespace IPodnik.Server.Infrastructure.Operations
{
    public interface IAgentTaskOperation : IScopedOperation
    {
        Task<List<AgentTask>> AddTaskAsync(AgentTaskOrderDto task);
        List<AgentTask> CreateTask(AgentTaskOrderDto task);
        Task<List<AgentTaskDto>> GetTasks(AgentTaskFilterDto filter);
        Task UpdateTask(AgentTaskUpdateDto task);
        Task<List<AgentTaskDto>> GetAllActiveTasks();
        Task BulkUpdateTasks(List<AgentTaskUpdateDto> tasks);
        Task EnqueueAllActiveTasks();
        Task UpdateTaskProgress(AgentTaskUpdateDto task);
    }

    public class AgentTaskOperation : IAgentTaskOperation
    {
        private readonly Lazy<IUnitOfWork> uow;
        private IAgentTaskRepository taskRepository => uow.Value.Repository<IAgentTaskRepository>();
        private readonly IServerTaskQueue taskQueue;

        public AgentTaskOperation(Lazy<IUnitOfWork> uow, IServerTaskQueue taskQueue)
        {
            this.uow = uow;
            this.taskQueue = taskQueue;
        }

        public async Task<List<AgentTask>> AddTaskAsync(AgentTaskOrderDto tasks)
        {
            if (tasks == null || !tasks.MachinesId.Any() || !tasks.TaskTypesId.Any() || tasks.UserProfileId == 0)
                throw new Exception("Missing important values");

            var result = await uow.Value.Repository<IAgentTaskRepository>().AddRangeAsync(CreateTask(tasks));
            return result.ToList();
        }

        public List<AgentTask> CreateTask(AgentTaskOrderDto task)
        {
            List<AgentTask> list = new List<AgentTask>();

            foreach (var machineId in task.MachinesId)
            {
                foreach (var taskTypeId in task.TaskTypesId)
                {
                    list.Add(new AgentTask
                    {
                        MachineId = machineId,
                        UserProfileId = task.UserProfileId,
                        Comment = task.Comment,
                        StartTime = task.StartTime,
                        TaskStatusId = (int)TaskStatusEnum.Pending,
                        CreateDate = DateTime.UtcNow,
                        TaskTypeId = taskTypeId
                    });
                }
            }

            return list;
        }

        public async Task<List<AgentTaskDto>> GetTasks(AgentTaskFilterDto filter)
        {
            if (filter == null)
                return new List<AgentTaskDto>();

            var tasks = await taskRepository.GetTasks(filter);
            return tasks?.Adapt<List<AgentTaskDto>>() ?? new List<AgentTaskDto>();
        }

        public async Task UpdateTask(AgentTaskUpdateDto task)
        {
            if (task == null)
            {
                return;
            }

            var result = await taskRepository.GetAsync(task.AgentTaskId);
            if (result == null)
            {
                return;
            }

            result.Updated = DateTime.UtcNow;

            if (task.NewStatus > TaskStatusEnum.NoChange)
            {
                result.TaskStatusId = (int)task.NewStatus;
            }

            if (task.Result != null)
            {
                result.Result = task.Result;
            }

            if (task.Comment != null)
            {
                result.Comment = task.Comment;
            }

            if (task.Progress > result.Progress)
            {
                result.Progress = task.Progress;
            }

            if (task.StatusMessage != null)
            {
                result.Result = task.StatusMessage;
            }

            await uow.Value.SaveChangesAsync();
        }

        public async Task<List<AgentTaskDto>> GetAllActiveTasks()
        {
            var tasks = await taskRepository.GetAllActiveTasks();
            return tasks.Adapt<List<AgentTaskDto>>();
        }

        public async Task BulkUpdateTasks(List<AgentTaskUpdateDto> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return;
            }

            var taskIds = tasks.Select(t => t.AgentTaskId).ToList();
            var existingTasks = await taskRepository.GetByIdsAsync(taskIds);

            if (existingTasks == null || !existingTasks.Any())
            {
                return;
            }

            var taskDict = existingTasks.ToDictionary(t => t.AgentTaskId);

            foreach (var task in tasks)
            {
                if (taskDict.TryGetValue(task.AgentTaskId, out var existingTask))
                {
                    existingTask.Updated = DateTime.UtcNow;
                    if (task.NewStatus > 0)
                    {
                        existingTask.TaskStatusId = (int)task.NewStatus;
                    }

                    if (task.Result != null)
                    {
                        existingTask.Result = task.Result;
                    }
                }
            }

            await uow.Value.SaveChangesAsync();
        }

        public async Task EnqueueAllActiveTasks()
        {
            var activeTasks = await GetAllActiveTasks();
            if (activeTasks.Any())
            {
                taskQueue.Add(activeTasks);
            }
        }

        public async Task UpdateTaskProgress(AgentTaskUpdateDto task)
        {
            if (task == null)
            {
                return;
            }

            var result = await taskRepository.GetAsync(task.AgentTaskId);
            if (result == null)
            {
                return;
            }

            result.Updated = DateTime.UtcNow;

            if (task.Result != null)
            {
                result.Result = task.Result;
            }

            if (task.Progress > result.Progress)
            {
                result.Progress = task.Progress;
                result.Result = task.StatusMessage;
            }

            await uow.Value.SaveChangesAsync();
        }

    }
}
