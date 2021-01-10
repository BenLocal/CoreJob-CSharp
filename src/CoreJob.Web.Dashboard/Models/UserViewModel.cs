using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJob.Web.Dashboard.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }

        public string UserDisplayName { get; set; }

        public string Password { get; set; }

        public string Redirect { get; set; }

        public string NewPassword { get; set; }

        public int Id { get; set; }
    }
}
