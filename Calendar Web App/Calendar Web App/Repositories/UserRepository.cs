using System.Security.Claims;
using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.ViewModels.AccountAccessViewModels;
using Calendar_Web_App.ViewModels.AccountSettingsViewModels;
using Microsoft.AspNetCore.Identity;

namespace Calendar_Web_App.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<UserRepository> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<User> GetCurrentUserAsync(ClaimsPrincipal User){
            try
            {
                _logger.LogInformation("Executing GetCurrentUserAsync operation in User Repository");

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogWarning("User {Username} does not exist", User.Identity.Name);
                }
                else
                {
                    _logger.LogInformation("User {Username} with Id {UserId} was successfully found", user.Name, user.Id);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to retrieve User {Username}", User.Identity.Name);
                throw;
            }
	    }


        public async Task<SignInResult> LoginUserAsync(LoginViewModel LoginModel)
        {
            try
            {
                _logger.LogInformation("Executing LoginUserAsync operation in User Repository");

                var result = await _signInManager.PasswordSignInAsync(LoginModel.Login, LoginModel.Password, true, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully logged in");
                }
                else
                {
                    _logger.LogWarning("User could not be logged in");
                }
                return result;
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured while trying to log in");
                throw;
            }

        }


        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel RegisterModel)
        {
            try
            {
                _logger.LogInformation("Executing RegisterUserAsync operation in UserRepository");

                //Create new user to be added
                var User = new User
                {
                    Name = RegisterModel.Name,
                    Email = RegisterModel.Email,
                    UserName = RegisterModel.Login,
                };

                //Try to add new user to database
                var result = await _userManager.CreateAsync(User, RegisterModel.Password);

                if(result.Succeeded)
                {
                    _logger.LogInformation("Registering User was successful");
                }
                else
                {
                    _logger.LogWarning("User could not be registered");
                }
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to register User");
                throw;
            }
        }



        public async Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal User, ChangePasswordViewModel ChangePasswordModel)
        {
            try
            {
                _logger.LogInformation("Executing ChangePasswordAsync operation in User Repository");

                //Get current user
                var CurrentUser = await GetCurrentUserAsync(User);

                //Try to change password in database
                var result = await _userManager.ChangePasswordAsync(CurrentUser, ChangePasswordModel.OldPassword, ChangePasswordModel.NewPassword);

                if(result.Succeeded)
                {
                    _logger.LogInformation("Changing User {Username} password was successful", CurrentUser.Name);
                }
                else
                {
                    _logger.LogWarning("Changing User {Username} password has failed", CurrentUser.Name);
                }
                return result;
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to Change User password");
                throw;
            }
        }



        public async void LogoutUserAsync()
        {
            try
            {
                _logger.LogInformation("Executing LogoutUserAsync operation in UserRepository");
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to Logout");
                throw;
            }
        }

        public async Task<IdentityResult> ChangeUsernameAsync(ClaimsPrincipal User, ChangeUsernameViewModel newUsername)
        {
            try
            {
                _logger.LogInformation("Executing ChangeUsernameAsync operation in User Repository");

                //Get current user
                var CurrentUser = await GetCurrentUserAsync(User);

                //Change username
                CurrentUser.UserName = newUsername.Username;

                //Update user in database
                var UpdateResult =  await _userManager.UpdateAsync(CurrentUser);

                if(UpdateResult.Succeeded)
                {
                    _logger.LogInformation("Updating Username was successful");
                }
                else
                {
                    _logger.LogWarning("Updating Username has failed");
                }
                return UpdateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to change Username");
                throw;
            }
        }

        public async Task<IdentityResult> ChangeEmailAsync(ClaimsPrincipal User, ChangeEmailViewModel newUsername)
        {
            try
            {
                _logger.LogInformation("Executing ChangeEmailAsync operation in UserRepository");

                //Get current user
                var CurrentUser = await GetCurrentUserAsync(User);

                //update user in database
                var result =  await _userManager.UpdateAsync(CurrentUser);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully updated {Username}'s email", CurrentUser.Name);
                }
                else
                {
                    _logger.LogWarning("Changing {Username}'s email has failed", CurrentUser.Name);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected Error occured while trying to Change Email");
                throw;
            }
        }

        public async Task<IdentityResult> ChangeNameAsync(ClaimsPrincipal User, ChangeNameViewModel newName)
        {
            try
            {
                _logger.LogInformation("Executing ChangeNameAsync in User Repository");

                //Retrieve current user from database
                var CurrentUser = await GetCurrentUserAsync(User);


                //Change Name
                CurrentUser.Name = newName.Name;


                //Update user in database
                var UpdateNameResult =  await _userManager.UpdateAsync(CurrentUser);

                if (UpdateNameResult.Succeeded)
                {
                    _logger.LogInformation("Changing User's Name was successful");
                }
                else
                {
                    _logger.LogWarning("Changing User's Name has failed");
                }
                return UpdateNameResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to change User's Name");
                throw;
            }
        }

        public async Task<IdentityResult> CloseAccountAsync(User User)
        {
            try
            {
                _logger.LogInformation("Executing CloseAccountAsync operation in User Repository");

                //Delete user from Db
                var CloseAccountResult =  await _userManager.DeleteAsync(User);

                if(CloseAccountResult.Succeeded)
                {
                    _logger.LogInformation("Closing {Username}'s account was successful", User.Name);
                }
                else
                {
                    _logger.LogWarning("Closing {Username}'s account has failed", User.Name);
                }
                return CloseAccountResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to Close {Username}'s account", User.Name);
                throw;
            }
        }

	}

    
}
