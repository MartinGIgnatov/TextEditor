using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TextEditorApi.Models;
using TextEditorApi.Services;

namespace TextEditorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegistration userForRegistration)
        {
            try
            {
                _authenticationService.Register(userForRegistration);
                return Ok();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return BadRequest(new
                {
                    Error = invalidOperationException.Message
                });
            }
            catch (Exception exception)
            {

                return new ObjectResult(new
                {
                    Error = exception.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        [HttpPost("login")]
        public IActionResult LogIn(UserForLogIn userForLogIn)
        {
            try
            {
                var result = _authenticationService.LogIn(userForLogIn);
                return Ok(result);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return BadRequest(new
                {
                    Error = invalidOperationException.Message
                });
            }
            catch (Exception exception)
            {

                return new ObjectResult(new
                {
                    Error = exception.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
