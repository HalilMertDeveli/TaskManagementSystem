using HMD.TaskManagement.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PriorityController : Controller
    {
        private readonly IMediator? mediator;

        public PriorityController(IMediator? mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> List()
        {
            var result = await this.mediator.Send(new PriorityListRequest());
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PriorityCreateRequest request)
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
                    ModelState.AddModelError("", result.ErrorMessage ?? "Ürüticiye başvur ");
                }
            }

            return View(request);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await this.mediator.Send(new PriorityDeleteRequest(id));

            return RedirectToAction("List");



        }
    }


}
