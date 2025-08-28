using System;
using System.Collections.Generic;

namespace IPodnik.Server.Infrastructure.Models
{
    public partial class Machine
    {
        public Machine()
        {
            AgentTasks = new HashSet<AgentTask>();
        }

        public int MachineId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Ram { get; set; }
        public int? Storage { get; set; }
        public string Ipaddress { get; set; }
        public int? HostSystemId { get; set; }

        public virtual HostSystem HostSystem { get; set; }
        public virtual ICollection<AgentTask> AgentTasks { get; set; }
    }
}
