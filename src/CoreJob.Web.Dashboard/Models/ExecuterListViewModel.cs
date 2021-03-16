using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJob.Web.Dashboard.Models
{
    public class ExecuterListViewModel
    {
        public int PageNum { get; set; }

        public int PageSize { get; set; }

        public string SearchExecutorName { get; set; }

        public string SearchRegistryName { get; set; }
    }
}
