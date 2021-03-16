using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoreJob.Web.Dashboard.Models
{
    public class JobViewModel
    {
        public bool IsNew { get; set; }

        public bool IsCopy { get; set; }

        public int JobId { get; set; }

        public string JobName { get; set; }

        public int ExecutorId { get; set; }

        public string Cron { get; set; }

        public string ExecutorHandler { get; set; }

        public string ExecutorParam { get; set; }

        public string CreateUser {get;set;}

        private IEnumerable<SelectListItem> _executorItems;
        public IEnumerable<SelectListItem> ExecutorItems
        {
            get
            {
                if (_executorItems == null)
                {
                    return null;
                }

                return _executorItems;
            }

            set
            {
                _executorItems = value;
            }
        }

        public int SelectorType { get; set; }

        private IEnumerable<SelectListItem> _selectorTypeItems;
        public IEnumerable<SelectListItem> SelectorTypeItems
        {
            get
            {
                if (_selectorTypeItems == null)
                {
                    return null;
                }

                return _selectorTypeItems;
            }

            set
            {
                _selectorTypeItems = value;
            }
        }
    }
}
