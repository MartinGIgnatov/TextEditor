using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TextEditorMVC.Services;
using TextEditorMVC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace TextEditorMVC.Controllers
{
    [Authorize]
    public class AuthenticationController : Controller
    {
        private TextEditorMVC.Services.IAuthenticationService _authenticationService;

        public AuthenticationController(TextEditorMVC.Services.IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Logging in an user with information given from <see cref="UserForLogin"/> model and show a message to inform the user.
        /// </summary>
        /// <param name="userForLogin"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLogin userForLogin)
        {
            ;
            try
            {
                var result = _authenticationService.Login(userForLogin);

                ClaimsIdentity identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, result),
                   new Claim(ClaimTypes.NameIdentifier, userForLogin.Username)

                }, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Message", new Message { Text = "Log in successful." });
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return RedirectToAction("Message", new Message { Text = invalidOperationException.Message });
            }
            catch (Exception exception)
            {
                return RedirectToAction("Message", new Message { Text = exception.Message });
            }
        }

        /// <summary>
        /// A form which is shown when the user want to log in.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult LoginForm()
        {
            return View();
        }

        /// <summary>
        /// Loggin out the user who is already logged in and show a message to inform the user.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Message", new Message { Text = "Logout successful" });
        }

        /// <summary>
        /// Show a massage to inform the user by using <see cref="Message"/ model.>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Message(Message message)
        {
            return View(message);
        }

        /// <summary>
        /// Register an user with information give from <see cref="UserForRegistration/> model.
        /// </summary>
        /// <param name="userForRegistration"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Register(UserForRegistration userForRegistration)
        {
            if (userForRegistration == null)
            {
                return View("Message", new Message { Text = "Unexpected error." });
            }

            try
            {
                _authenticationService.Register(userForRegistration);
                return View("Message", new Message { Text = "Register successful." });
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return View("Message", new Message { Text = invalidOperationException.Message });
            }
            catch (Exception exception)
            {
                _ = exception;
                return View("Message", new Message { Text = exception.Message });
            }
        }

        /// <summary>
        /// A form which is shown when the user want to register.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult RegisterForm()
        {
            return View();
        }

        /// <summary>
        /// A form for text in which user got informed.
        /// </summary>
        /// <returns></returns>
        public IActionResult TextForm()
        {
            return View();
        }
    }
}