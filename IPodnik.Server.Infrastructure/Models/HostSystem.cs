using System;
using System.Collections.Generic;

namespace IPodnik.Server.Infrastructure.Models
{
    public partial class HostSystem
    {
        public HostSystem()
        {
            Machines = new HashSet<Machine>();
        }

        public int HostSystemId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Machine> Machines { get; set; }
    }
}
