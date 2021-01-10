using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJob.Web.Dashboard.Models
{
    public class DashboardViewModel
    {
        public int JobCount { get; set; }

        public int ExecuterCount { get; set; }

        public int RunTimes { get; set; }

        public string HasData { get; set; }

        public List<string> DataTimeList { get; set; }

        public List<int> SuccessCountList { get; set; }

        public List<int> FailCountList { get; set; }

        public List<int> RunningCountList { get; set; }
    }
}
