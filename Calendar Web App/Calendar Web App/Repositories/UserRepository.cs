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

        public UserRepository(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> GetCurrentUserAsync(ClaimsPrincipal User){
	        var user = await _userManager.GetUserAsync(User);
            return user;
	    }


        public async Task<SignInResult> LoginUserAsync(LoginViewModel LoginModel)
        {
            //return result of signing in via password
            return await _signInManager.PasswordSignInAsync(LoginModel.Login, LoginModel.Password, true, false);
        }


        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel RegisterModel)
        {
            //Create new user to be added
            var User = new User
            {
                Name = RegisterModel.Name,
                Email = RegisterModel.Email,
                UserName = RegisterModel.Login,
            };

            //Try to add new user to database
            return await _userManager.CreateAsync(User, RegisterModel.Password);
            
        }
        public async Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal User, ChangePasswordViewModel ChangePasswordModel)
        {
            //Get current user
            var CurrentUser = await _userManager.GetUserAsync(User);



            //Try to change password in database
            return await _userManager.ChangePasswordAsync(CurrentUser, ChangePasswordModel.OldPassword, ChangePasswordModel.NewPassword);
        }



        public async void LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangeUsernameAsync(ClaimsPrincipal User, ChangeUsernameViewModel newUsername)
        {
            //Get current user
            var CurrentUser = await _userManager.GetUserAsync(User);

            //Change username
            CurrentUser.UserName = newUsername.Username;

            //Update user in database
            return await _userManager.UpdateAsync(CurrentUser);
        }

        public async Task<IdentityResult> ChangeEmailAsync(ClaimsPrincipal User, ChangeEmailViewModel newUsername)
        {
	        //Get current user
	        var CurrentUser = await _userManager.GetUserAsync(User);


            //change email
	        CurrentUser.Email = newUsername.Email;


            //update user in database
	        return await _userManager.UpdateAsync(CurrentUser);
        }

        public async Task<IdentityResult> ChangeNameAsync(ClaimsPrincipal User, ChangeNameViewModel newName)
        {
            //Retrieve current user from database
            var CurrentUser = await _userManager.GetUserAsync(User);


            //Change Name
            CurrentUser.Name = newName.Name;

            //Update user in database
            return await _userManager.UpdateAsync(CurrentUser);
        }

        public async Task<IdentityResult> CloseAccountAsync(User User)
        {
            //Delete user from Db
            return await _userManager.DeleteAsync(User);
        }

	}

    
}
