using IPodnik.Agent.Infrastructure.Modules;
using IPodnik.Agent.Infrastructure.Modules.LogModule;
using IPodnik.Agent.Infrastructure.Modules.TestCasesModule;
using IPodnik.Agent.Infrastructure.Modules.TestFailMidway;
using IPodnik.Agent.Infrastructure.Modules.TestHang;
using IPodnik.Agent.Infrastructure.Modules.TestInvalidInput;
using IPodnik.Agent.Infrastructure.Modules.ZipModule;
using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Enums;


namespace IPodnik.Agent.Infrastructure.Operations
{
    public interface IStrategyProvider : ISingletonHelper
    {
        IJobStrategy GetStrategy(TaskTypeEnum taskType);
    }

    public class StrategyProvider : IStrategyProvider
    {
        private readonly Dictionary<TaskTypeEnum, Func<IJobStrategy>> strategies;

        public StrategyProvider(IAgentTaskProgressQueue statusQueue)
        {
            strategies = new Dictionary<TaskTypeEnum, Func<IJobStrategy>>
            {
                { TaskTypeEnum.ZipInstaller, () => new ZipStrategy(statusQueue) },
                { TaskTypeEnum.Logger, () => new LogStrategy() },
                { TaskTypeEnum.TestFail, () => new TestFailStrategy(statusQueue) },
                { TaskTypeEnum.TestFailMidway, () => new TestFailMidwayStrategy(statusQueue) },
                { TaskTypeEnum.TestHang, () => new TestHangStrategy(statusQueue) },
                { TaskTypeEnum.TestInvalidInput, () => new TestInvalidInputStrategy(statusQueue) }
            };
        }

        public IJobStrategy GetStrategy(TaskTypeEnum taskType)
        {
            if (strategies.TryGetValue(taskType, out var strategy))
            {
                return strategy();
            }

            throw new InvalidOperationException($"No strategy found for TaskTypeId {taskType}");
        }
    }

}
