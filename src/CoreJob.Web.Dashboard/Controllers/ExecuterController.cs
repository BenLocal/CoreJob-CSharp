using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Web.Dashboard.Models;
using CoreJob.Web.Dashboard.Services.Command.Executer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreJob.Web.Dashboard.Controllers
{
    public class ExecuterController : BaseController
    {
        private readonly IMediator _mediator;

        public ExecuterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            return View("Edit", new ExecuterViewModel()
            {
                IsNew = true
            });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _mediator.Send(new ExecuterEditCmd() { ExecuterId = id });
            return View("Edit", result);
        }

        public async Task<IActionResult> ExecuterList([FromForm] ExecuterListViewModel model)
        {
            var list = await _mediator.Send(new GetExecuterListCmd()
            {
                PageIndex = model.PageNum,
                PageSize = model.PageSize
            });
            return Ok(list);
        }

        public async Task<IActionResult> Save([FromForm] ExecuterViewModel model)
        {
            var reslut = await _mediator.Send(new SaveExecuterCmd() { VM = model });
            return Ok(reslut);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var reslut = await _mediator.Send(new DeleteExecuterCmd() { ExecuterId = id });
            return Ok(reslut);
        }

        public async Task<IActionResult> DeleteHost([FromRoute] int id)
        {
            var reslut = await _mediator.Send(new DeleteExecuterHostCmd() { Id = id });
            return Ok(reslut);
        }
    }
}
