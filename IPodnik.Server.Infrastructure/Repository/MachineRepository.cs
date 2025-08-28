using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Server.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace IPodnik.Server.Infrastructure.Repository
{
    public interface IMachineRepository : IGaiaRepository<Machine>
    {
        Task<List<Machine>> GetMachinesAsync(MachineFilter filter);
        Task<List<Machine>> GetAllMachinesAsync();
        Task<Machine> GetMachineByName(string name);
        Task<Machine> GetByIdAsync(int id);
    }


    public class MachineRepository : GaiaRepository<Machine>, IMachineRepository
    {
        public MachineRepository(IPodnikGaiaContext dbContext) : base(dbContext) { }

        public async Task<List<Machine>> GetMachinesAsync(MachineFilter filter)
        {
            if (filter == null)
                return new List<Machine>();

            var query = context.Machines.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(m => m.Name.Contains(filter.Name));

            if (filter.HostSystemId.HasValue)
                query = query.Where(m => m.HostSystemId == filter.HostSystemId.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Machine>> GetAllMachinesAsync()
        {
            return await context.Machines.ToListAsync();
        }

        public async Task<Machine> GetMachineByName(string name)
        {
            return string.IsNullOrEmpty(name)
                ? null
                : await context.Machines.FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<Machine> GetByIdAsync(int id)
        {
            return await context.Machines
                .FirstOrDefaultAsync(m => m.MachineId == id);
        }
    }

}
