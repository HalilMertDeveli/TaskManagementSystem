using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.UI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> List(string? s, int activePage = 1)
        {
            ViewBag.s = s;
            ViewBag.Active = "Members";
            var result = await this.mediator.Send(new MemberListPagedRequest(activePage, s));
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var result = await this.mediator.Send(new MemberResetPasswordRequest(id));
            if (!result.IsSuccess || result.Data is null)
                return NotFound();

            return TemporaryPasswordView(result.Data);
        }

        // The generated password is rendered directly in the POST response (no redirect, no TempData)
        // so it is shown exactly once and not persisted anywhere in clear text.
        private IActionResult TemporaryPasswordView(TemporaryPasswordDto dto)
        {
            ViewBag.Active = "Members";
            Response.Headers.CacheControl = "no-store, no-cache";
            Response.Headers.Pragma = "no-cache";
            return View("TemporaryPassword", dto);
        }


        public async Task<IActionResult> Update(int id)
        {
            var result = await this.mediator.Send(new MemberGetByIdRequest(id));
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MemberUpdateRequest request)
        {
            var result = await this.mediator.Send(request);
            if (result.IsSuccess)
            {
                return RedirectToAction("List");
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
                    ModelState.AddModelError("", result.ErrorMessage ?? "Sistemsel hata");
                }
            }


            return View(request);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MemberCreateRequest request)
        {
            var result = await this.mediator.Send(request);

            if (result.IsSuccess && result.Data is not null)
            {
                return TemporaryPasswordView(result.Data);
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


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.mediator.Send(new MemberDeleteRequest(id));
            return RedirectToAction("List");
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


            return View(request);


        }

    }



}

