using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoreJob.Web.Dashboard.Models
{
    public class JobListViewModel
    {
        public int PageNum { get; set; }

        public int PageSize { get; set; }

        public string SearchJobName { get; set; }

        public string SearchCreateUser { get; set; }

        public int SearchStatus { get; set; }

        public int SearchExecutorId { get; set; }

        private IEnumerable<SelectListItem> _executorItems;
        public IEnumerable<SelectListItem> ExecutorItems
        {
            get
            {
                if (_executorItems == null)
                {
                    return default;
                }

                return _executorItems;
            }

            set
            {
                _executorItems = value;
            }
        }
    }
}
