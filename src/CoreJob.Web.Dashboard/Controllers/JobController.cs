using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreJob.Web.Dashboard.Models;
using CoreJob.Web.Dashboard.Services.Command.Job;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace CoreJob.Web.Dashboard.Controllers
{
    public class JobController : BaseController
    {
        private readonly IMediator _mediator;

        public JobController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new JobListIndexCmd());
            return View(result);
        }

        public IActionResult Cron() => View();

        public async Task<IActionResult> New()
        {
            var result = await _mediator.Send(new GetJobEditCmd()
            {
                IsNew = true,
                IsCopy = false
            });
            return View("Edit", result);
        }

        public async Task<IActionResult> Copy(int id)
        {
            var result = await _mediator.Send(new GetJobEditCmd()
            {
                JobId = id,
                IsNew = false,
                IsCopy = true
            });
            return View("Edit", result);
        }

        public async Task<IActionResult> Trigger(int id)
        {
            var result = await _mediator.Send(new GetTriggerJobCmd() { JobId = id });
            return View("Trigger", result);
        }

        [HttpPost]
        public async Task<IActionResult> Trigger([FromForm] JobTriggerViewModel model)
        {
            var reslut = await _mediator.Send(new TriggerJobCmd() { VM = model });
            return Ok(reslut);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _mediator.Send(new GetJobEditCmd()
            {
                JobId = id,
                IsNew = false,
                IsCopy = false
            });
            return View("Edit", result);
        }

        [HttpPost("joblist")]
        public async Task<IActionResult> JobList([FromForm] JobListViewModel model)
        {
            var list = await _mediator.Send(new GetJobListCmd()
            { 
                PageIndex = model.PageNum,
                PageSize = model.PageSize,
                SearchJobName = model.SearchJobName,
                SearchExecutorId = model.SearchExecutorId,
                SearchCreateUser = model.SearchCreateUser,
                SearchStatus = model.SearchStatus
            });
            return Ok(list);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromForm] JobViewModel model, bool trigger)
        {
            model.CreateUser = GetCurrentUserName();
            var reslut = await _mediator.Send(new SaveJobCmd() { VM = model, Trigger = trigger });
            return Ok(reslut);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var reslut = await _mediator.Send(new DeleteJobCmd() { JobId = id });
            return Ok(reslut);
        }

        public async Task<IActionResult> Resume(int id)
        {
            var reslut = await _mediator.Send(new ResumeOrPauseJobCmd()
            {
                JobId = id,
                Resume = true
            });
            return Ok(reslut);
        }

        public async Task<IActionResult> Pause(int id)
        {
            var reslut = await _mediator.Send(new ResumeOrPauseJobCmd()
            {
                JobId = id,
                Pause = true
            });
            return Ok(reslut);
        }

        [HttpPost]
        public async Task<IActionResult> Cron([FromBody] CronViewModel model)
        {
            var cron = model?.Text;
            if (string.IsNullOrEmpty(cron))
            {
                return await Task.FromResult(Ok(new { description = "", next = new object[0] }));
            }


            string desc = "格式错误";
            try
            {
                desc = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cron, new CronExpressionDescriptor.Options()
                { 
                    Locale = "zh-cn"
                });
            }
            catch
            {
                // TODO
            }

            List<string> nextDates = new List<string>();
            try
            {
                var qce = new CronExpression(cron);
                DateTime dt = DateTime.Now;
                for (int i = 0; i < 10; i++)
                {
                    var next = qce.GetNextValidTimeAfter(dt);
                    if (next == null)
                        break;
                    nextDates.Add(next.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    dt = next.Value.LocalDateTime;
                }
            }
            catch
            {
                // TODO
            }

            return await Task.FromResult(Ok(new { description = desc, next = nextDates }));
        }
    }
}
