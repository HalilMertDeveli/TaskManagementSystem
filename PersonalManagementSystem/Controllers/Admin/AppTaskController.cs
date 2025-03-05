using HMD.TaskManagement.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class AppTaskController : Controller
    {
        private readonly IMediator mediator;

        public AppTaskController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> List()
        {
            var result = await this.mediator.Send(new AppTaskListRequest());
            return View(result.Data);
        }
    }
}
