using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Server.Infrastructure.Operations;
using Microsoft.Extensions.Hosting;

namespace IPodnik.Server.Infrastructure.Services
{
    public class AgentMonitorService : BackgroundService
    {
        private readonly IAgentMonitor monitor;
        private readonly Factory<IMachineOperation> machineOperationFactory;

        public AgentMonitorService(IAgentMonitor monitor, Factory<IMachineOperation> machineOperationFactory)
        {
            this.monitor = monitor;
            this.machineOperationFactory = machineOperationFactory;
        }

        public async Task InitAgentQueue()
        {
            var machineOperation = machineOperationFactory.Create();
            var machines = await machineOperation.GetAllMachines();
            await monitor.AddMachines(machines);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitAgentQueue();

            while (!stoppingToken.IsCancellationRequested)
            {
                var inactiveAgents = monitor.GetInactiveAgents();

                foreach (var machineName in inactiveAgents)
                {
                    //TODO AKCE, Oznameni uzivatelum/ zapsani nekam/ zarazeni machiny do neaktivni kategorie
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
