using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.UI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers.Member
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class NotificationController : Controller
    {
        private readonly IMediator mediator;

        public NotificationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> List()
        {
            ViewBag.Active = "Notification";
            var result = await this.mediator.Send(new NotificationListByUserIdRequest(User.GetUserId()));
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id)
        {
            var result = await this.mediator.Send(new NotificationUpdateRequest(id, User.GetUserId()));
            if (!result.IsSuccess)
                return NotFound();

            return RedirectToAction("List");
        }
    }
}
