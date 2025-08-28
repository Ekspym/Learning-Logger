using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Infrastructure.UnitOfWork;
using IPodnik.Server.Infrastructure.Repository;
using Mapster;

namespace IPodnik.Server.Infrastructure.Operations
{
    public interface IMachineOperation : IScopedOperation
    {
        Task<List<MachineInfoDto>> GetMachinesAsync(MachineFilter filter);
        Task<List<MachineInfoDto>> GetAllMachines();
        Task<MachineInfoDto> GetMachineByName(string name);
        Task<MachineInfoDto> GetByIdsAsync(int machineId);
    }


    public class MachineOperation : IMachineOperation
    {
        private readonly Lazy<IUnitOfWork> uow;
        private IMachineRepository machineRepository => uow.Value.Repository<IMachineRepository>();

        public MachineOperation(Lazy<IUnitOfWork> uow)
        {
            this.uow = uow;
        }

        public async Task<List<MachineInfoDto>> GetMachinesAsync(MachineFilter filter)
        {
            if (filter == null)
            {
                return [];
            }

            var machines = await machineRepository.GetMachinesAsync(filter);
            return machines.Count > 0 ? machines.Adapt<List<MachineInfoDto>>() : new List<MachineInfoDto>();
        }

        public async Task<List<MachineInfoDto>> GetAllMachines()
        {
            var machines = await machineRepository.GetAllMachinesAsync();
            return machines.Count > 0 ? machines.Adapt<List<MachineInfoDto>>() : new List<MachineInfoDto>();
        }

        public async Task<MachineInfoDto> GetMachineByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var machine = await machineRepository.GetMachineByName(name);
            return machine?.Adapt<MachineInfoDto>();
        }

        public async Task<MachineInfoDto> GetByIdsAsync(int machineId)
        {
            var machines = await machineRepository.GetByIdAsync(machineId);
            return machines.Adapt<MachineInfoDto>();
        }
    }
}
