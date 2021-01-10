using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Store
{
    public class DashboardUser
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HashPassword { get; set; }

        public string DisplayName { get; set; }

        public bool Disabled { get; set; }

        public DateTime InTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
