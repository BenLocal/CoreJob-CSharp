using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Web.Dashboard.Services.Command;
using CoreJob.Web.Dashboard.Services.Command.LogLogic;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreJob.Web.Dashboard.Controllers
{
    public class LogsController : BaseController
    {
        private readonly IMediator _mediator;

        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index(int? id)
        {
            ViewData["JobId"] = id;

            return View();
        }

        public async Task<IActionResult> LogList([FromRoute]int id,
            [FromForm] int pageNum, [FromForm] int pageSize)
        {
            var list = await _mediator.Send(new GetLogListCmd()
            { 
                JobId = id,
                PageIndex = pageNum,
                PageSize = pageSize
            });
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> LogInfo(int id)
        {
            var result = await _mediator.Send(new GetLogInfoCmd()
            {
                LogId = id
            });
            return Ok(result);
        }
    }
}
