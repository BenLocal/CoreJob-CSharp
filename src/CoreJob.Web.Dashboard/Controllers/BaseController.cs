using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreJob.Web.Dashboard.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected int GetCurrentUserId()
        {
            return Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        protected string GetCurrentUserName()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
