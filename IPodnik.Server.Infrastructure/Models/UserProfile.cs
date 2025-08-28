using System;
using System.Collections.Generic;

namespace IPodnik.Server.Infrastructure.Models
{
    public partial class UserProfile
    {
        public UserProfile()
        {
            AgentTasks = new HashSet<AgentTask>();
        }

        public int UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<AgentTask> AgentTasks { get; set; }
    }
}
