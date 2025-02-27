using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HMD.TaskManagement.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator mediator;

        public AccountController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)//record
        {
            var response= await  this.mediator.Send(request);
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            return View();
        }
    }
}
