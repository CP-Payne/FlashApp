using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using ErrorOr;
using FlashApp.Dtos.Authentication;
using FlashApp.Interfaces.Services;
using FlashApp.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashApp.Controllers
{
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ErrorOr<AuthenticationResult> authResult = await _authService.CreateUser(
                registerDto.Username!,
                registerDto.Email!,
                registerDto.Password!
            );

            return authResult.Match(
                authResult =>
                {
                    return Ok(
                        new NewUserDto
                        {
                            Email = authResult.User.Email!,
                            Username = authResult.User.UserName!,
                            Token = authResult.Token,
                        }
                    );
                },
                errors => Problem(errors)
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ErrorOr<AuthenticationResult> authResult = await _authService.AuthenticateUser(
                loginDto.Email!,
                loginDto.Password!
            );

            return authResult.Match(
                authResult =>
                {
                    return Ok(
                        new NewUserDto
                        {
                            Email = authResult.User.Email!,
                            Username = authResult.User.UserName!,
                            Token = authResult.Token,
                        }
                    );
                },
                errors => Problem(errors)
            );
        }
    }
}
