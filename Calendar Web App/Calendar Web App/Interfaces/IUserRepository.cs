using System.Security.Claims;
using Calendar_Web_App.Models;
using Calendar_Web_App.ViewModels.AccountAccessViewModels;
using Calendar_Web_App.ViewModels.AccountSettingsViewModels;
using Microsoft.AspNetCore.Identity;

namespace Calendar_Web_App.Interfaces
{
    public interface IUserRepository
    {

        Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal User, ChangePasswordViewModel ChangePasswordModel);
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel RegisterModel);
        Task<SignInResult> LoginUserAsync(LoginViewModel LoginModel);
        Task<IdentityResult> ChangeUsernameAsync(ClaimsPrincipal User, ChangeUsernameViewModel newUsername);
        Task<IdentityResult> ChangeEmailAsync(ClaimsPrincipal User, ChangeEmailViewModel newUsername);
        Task<IdentityResult> ChangeNameAsync(ClaimsPrincipal User, ChangeNameViewModel newName);
		Task<User> GetCurrentUserAsync(ClaimsPrincipal User);

        Task<IdentityResult> CloseAccountAsync(User User);
		void LogoutUserAsync();
    }
}