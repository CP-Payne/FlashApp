using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using FlashApp.Interfaces.Services;
using FlashApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlashApp.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        public AuthenticationService(
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signinManager
        )
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signinManager = signinManager;
        }

        public async Task<ErrorOr<AuthenticationResult>> AuthenticateUser(
            string email,
            string password
        )
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x =>
                x.Email == email.ToLower()
            );

            if (user is null)
            {
                return Error.Unauthorized(description: "Invalid login credentials.");
            }

            var result = await _signinManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
            {
                return Error.Unauthorized(description: "Invalid login credentials.");
            }

            var token = _tokenService.CreateToken(user);

            return new AuthenticationResult(user, token);
        }

        public async Task<ErrorOr<AuthenticationResult>> CreateUser(
            string username,
            string email,
            string password
        )
        {
            var appUser = new AppUser { UserName = username, Email = email };

            var createdUser = await _userManager.CreateAsync(appUser, password);

            if (!createdUser.Succeeded)
            {
                return createdUser
                    .Errors.Select(e => Error.Validation(e.Code, e.Description))
                    .ToList();
            }

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
            if (!roleResult.Succeeded)
            {
                return roleResult
                    .Errors.Select(e => Error.Unexpected(e.Code, e.Description))
                    .ToList();
            }

            var token = _tokenService.CreateToken(appUser);

            return new AuthenticationResult(appUser, token);
        }
    }
}
