using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Server.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace IPodnik.Server.Infrastructure.Repository
{
    public interface IAgentTaskRepository : IGaiaRepository<AgentTask>
    {
        Task AddAsyncBulk(List<AgentTask> tasks);
        Task<List<AgentTask>> GetTasksForMachine(int machineId);
        Task<List<AgentTask>> GetTasks(AgentTaskFilterDto filter);
        Task<List<AgentTask>> GetAllActiveTasks();
        Task<List<AgentTask>> GetByIdsAsync(List<int> ids);
    }

    public class AgentTaskRepository : GaiaRepository<AgentTask>, IAgentTaskRepository
    {
        public AgentTaskRepository(IPodnikGaiaContext dbContext) : base(dbContext) { }

        public async Task AddAsyncBulk(List<AgentTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return;
            }

            context.AddRange(tasks);
            await context.SaveChangesAsync();
        }

        public async Task<List<AgentTask>> GetTasksForMachine(int machineId)
        {
            return await context.AgentTasks
                .Where(j => j.MachineId == machineId)
                .ToListAsync();
        }

        public async Task<List<AgentTask>> GetTasks(AgentTaskFilterDto filter)
        {
            if (filter == null)
            {
                return new List<AgentTask>();
            }

            var query = context.AgentTasks
                .Include(task => task.Machine)
                .AsQueryable();

            if (filter.AgentTaskId.HasValue)
            {
                query = query.Where(task => task.AgentTaskId == filter.AgentTaskId);
            }

            if (filter.TaskStatus.HasValue)
            {
                query = query.Where(task => task.TaskStatusId == (int)filter.TaskStatus);
            }

            if (filter.MachineId.HasValue)
            {
                query = query.Where(task => task.MachineId == filter.MachineId);
            }

            if (!string.IsNullOrEmpty(filter.MachineName))
            {
                query = query.Where(task => task.Machine.Name == filter.MachineName);
            }

            if (filter.TaskTypeId.HasValue)
            {
                query = query.Where(task => task.TaskTypeId == filter.TaskTypeId);
            }

            if (filter.UserProfileId.HasValue)
            {
                query = query.Where(task => task.UserProfileId == filter.UserProfileId);
            }


            return await query.ToListAsync();
        }

        public async Task<List<AgentTask>> GetAllActiveTasks()
        {
            return await context.AgentTasks
                                .Where(task => task.TaskStatusId != (int)TaskStatusEnum.Completed)
                                .Include(task => task.Machine)
                                .ToListAsync();
        }

        public async Task<List<AgentTask>> GetByIdsAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<AgentTask>();
            }

            return await context.AgentTasks
                .Where(t => ids.Contains(t.AgentTaskId))
                .ToListAsync();
        }

    }
}
