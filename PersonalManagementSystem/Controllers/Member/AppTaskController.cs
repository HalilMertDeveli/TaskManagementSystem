using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.UI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers.Member
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class AppTaskController : Controller
    {
        private readonly IMediator mediator;

        public AppTaskController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> List(string? s, int activePage = 1)
        {
            var userId = User.GetUserId();
            ViewBag.s = s;
            ViewBag.Active = "AppTask";
            var result = await this.mediator.Send(new AppTaskListByUserIdRequest(activePage, s, userId));
            return View(result);
        }

        public async Task<IActionResult> ReportList(int id)
        {
            var appTaskInfo = await this.mediator.Send(new AppTaskGetByIdRequest(id));

            // Members may only see reports of tasks assigned to them.
            if (appTaskInfo.Data is null || appTaskInfo.Data.AppUserId != User.GetUserId())
                return NotFound();

            ViewBag.TaskTitle = appTaskInfo.Data.Title;
            ViewBag.TaskId = appTaskInfo.Data.Id;
            var result = await this.mediator.Send(new TaskReportGetByTaskIdRequest(id));

            return View(result.Data);
        }

        public async Task<IActionResult> CreateReport(int taskId)
        {
            var appTaskInfo = await this.mediator.Send(new AppTaskGetByIdRequest(taskId));
            if (appTaskInfo.Data is null || appTaskInfo.Data.AppUserId != User.GetUserId())
                return NotFound();

            ViewBag.Active = "AppTask";
            return View(new TaskReportCreateRequest("", "", taskId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(TaskReportCreateRequest request)
        {
            ViewBag.Active = "AppTask";
            // The acting user always comes from the auth cookie, never from the form.
            request = request with { UserId = User.GetUserId() };

            var result = await this.mediator.Send(request);
            if (result.IsSuccess)
            {
                return RedirectToAction("ReportList", new { id = request.TaskId });
            }

            if (result.Errors?.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(request);
            }

            return NotFound();
        }

        public async Task<IActionResult> UpdateReport(int id)
        {
            var result = await this.mediator.Send(new TaskReportGetByIdRequest(id, User.GetUserId()));
            if (result.Data is null)
                return NotFound();

            return View(new TaskReportUpdateRequest(result.Data.Id, result.Data.Detail, result.Data.Definition, result.Data.AppTaskId));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReport(TaskReportUpdateRequest request)
        {
            ViewBag.Active = "AppTask";
            request = request with { UserId = User.GetUserId() };

            var result = await this.mediator.Send(request);
            if (result.IsSuccess)
            {
                return RedirectToAction("ReportList", new { id = request.TaskId });
            }

            if (result.Errors?.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(request);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReport(int id, int taskId)
        {
            var result = await this.mediator.Send(new TaskReportDeleteRequest(id, User.GetUserId()));
            if (!result.IsSuccess)
                return NotFound();

            return RedirectToAction("ReportList", new { id = taskId });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var result = await this.mediator.Send(new AppTaskCompleteRequest(id, User.GetUserId()));
            if (!result.IsSuccess)
                return NotFound();

            return RedirectToAction("List");
        }


    }
}
