using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.UI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers.Member
{
    [Area("Member")]
    [Authorize(Roles = "Member")]
    public class UserController : Controller
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> UserDetail()
        {
            ViewBag.Active = "Profil";
            var userId = User.GetUserId();
            var updated = await this.mediator.Send(new UserDetailRequest(userId));
            if (updated.Data is null)
            {
                ModelState.AddModelError("", updated.ErrorMessage ?? "Kullanici bilgisi bulunamadi.");
                return View(new UserDetailUpdateRequest(userId, string.Empty, string.Empty, null));
            }

            // The password field is always rendered empty; the current password is never sent to the browser.
            return View(new UserDetailUpdateRequest(updated.Data.Id, updated.Data.Name, updated.Data.Surname, null));
        }

        [HttpPost]
        public async Task<IActionResult> UserDetail(UserDetailUpdateRequest request)
        {
            ViewBag.Active = "Profil";

            // Users can only edit their own profile: the id comes from the auth cookie, not from the form.
            request = request with { Id = User.GetUserId() };

            var result = await this.mediator.Send(request);

            if (result.IsSuccess)
            {
                return RedirectToAction("UserDetail");
            }
            else
            {
                if (result.Errors?.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage ?? "Sistemsel bir hata oluştu, üreticinize başvurun");
                }
            }


            return View(request with { Password = null });


        }
    }
}
