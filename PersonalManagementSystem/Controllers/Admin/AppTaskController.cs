using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HMD.TaskManagement.UI.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppTaskController : Controller
    {
        private readonly IMediator mediator;

        public AppTaskController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        public async Task<IActionResult> List(string? s, int activePage = 1)
        {
            ViewBag.s = s;
            ViewBag.Active = "AppTask";
            var result = await this.mediator.Send(new AppTaskListRequest(activePage, s));
            return View(result);
        }
    }
}
