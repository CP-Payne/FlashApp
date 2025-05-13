using ErrorOr;
using FlashApp.Models;

namespace FlashApp.Interfaces.Services
{
    public record AuthenticationResult(AppUser User, string Token);

    public interface IAuthenticationService
    {
        Task<ErrorOr<AuthenticationResult>> CreateUser(
            string username,
            string email,
            string password
        );
        Task<ErrorOr<AuthenticationResult>> AuthenticateUser(string email, string password);
    }
}
