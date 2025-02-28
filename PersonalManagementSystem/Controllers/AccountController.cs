using System.Security.Claims;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            return View(new LoginRequest("",""));
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)//record
        {
            var result = await this.mediator.Send(request);
            if (result.IsSuccess && result.Data !=null)
            {
                await SetAuthCookie(result.Data,request.RememberMe);
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                if (result.Errors != null && result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage ?? "Bilinmeyen bir hata oluştu");

                }
                return View(request);
            }




        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private async Task SetAuthCookie(LoginResponseDto dto,bool RememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim("Name", dto.Name),
                new Claim("SurName", dto.Surname),
                new Claim(ClaimTypes.Role, dto.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),//time
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = RememberMe,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
