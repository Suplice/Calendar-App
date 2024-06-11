using Calendar_Web_App.Interfaces;
using Calendar_Web_App.ViewModels.AccountAccessViewModels;
using Calendar_Web_App.ViewModels.AccountSettingsViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Calendar_Web_App.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
 

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }




        [HttpPost]
		public async Task<IActionResult> Login(LoginViewModel UserLoginModel)
        {
	        
			if (!ModelState.IsValid)
            {
                //Model invalid - action returns login view with old model as parameter
                return View(UserLoginModel);
            }


         
            //validating user data
            var UserLoginResult = await _userRepository.LoginUserAsync(UserLoginModel);


            //data is correct, log in and proceed to Calendar view
            if (UserLoginResult.Succeeded)
            {
	            return RedirectToAction("Calendar", "Calendar");
			}


			//Account is on lockout
			if (UserLoginResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account is on Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt, Check your login or password");
            }

            ViewBag.ErrorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;

			//validation is incorrect, return login view with odl model
			return View(UserLoginModel);
        }



        
        [Authorize]
        [HttpPost]
		public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                _userRepository.LogoutUserAsync();
                return Ok();
            }
            return BadRequest();
        }




        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }




        [HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel NewUserModel)
        {
            //Checking whether model state is valid
            if (!ModelState.IsValid)
            {
                //Model state is invalid - action returns to Register view with old model as parameter
                return View(NewUserModel);
            }

            
            //validating User data
            var UserRegisterResult = await _userRepository.RegisterUserAsync(NewUserModel);



            if (UserRegisterResult.Succeeded)
            {
                //validation succeeded - redirect to Login action
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //Validation is incorrect - add all errors
                foreach (var error in UserRegisterResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ViewBag.ErrorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;

				//return to register view with old model as parameter
				return View(NewUserModel);
            }



        }


        [Authorize]
        [HttpPost]
		public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel NewPasswordModel)
        {

            //Checking whether model is valid
            if (!ModelState.IsValid)
            {
				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
				);
				return BadRequest(errors);
			}


            //validating user data
            var UserChangePasswordResult = await _userRepository.ChangePasswordAsync(HttpContext.User, NewPasswordModel);

            if (UserChangePasswordResult.Succeeded)
            {
                return Ok();
            }
            else
			{


				//Validation failed, return all errors
				foreach (var error in UserChangePasswordResult.Errors)
				{
                    if (error.Description == "Incorrect password.")
                    {
                        ModelState.AddModelError("OldPassword", error.Description);
                    }
                    else {
                        ModelState.AddModelError("NewPassword", error.Description);
                    }
				}

				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
				);

				return BadRequest(errors);
			}


		}

        [HttpPost]
        [Authorize]
		public async Task<IActionResult> UpdateUsername(ChangeUsernameViewModel newUsernameModel)
        {
            //return BadRequest if ModelState is invalid
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                return BadRequest(errors);
            }

            //Validate changing username
            var UserChangeUsernameResult = await _userRepository.ChangeUsernameAsync(HttpContext.User, newUsernameModel);


            //Check validation result
            if (UserChangeUsernameResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                //Validation failed, return all errors
                foreach (var error in UserChangeUsernameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Error: {error.Description}");
                }

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

                return BadRequest(errors);
            }
        }

        [HttpPost]
        [Authorize]

		public async Task<IActionResult> UpdateEmail(ChangeEmailViewModel newEmailModel)
        {

	        //return BadRequest if ModelState is invalid
			if (!ModelState.IsValid)
            {
	            var errors = ModelState.ToDictionary(
		            kvp => kvp.Key,
		            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
	            );
	            return BadRequest(errors);
			}

			//Validate changing Email
			var UserChangeEmailResult = await _userRepository.ChangeEmailAsync(HttpContext.User, newEmailModel);


			//Check validation result
			if (UserChangeEmailResult.Succeeded)
            {
                return Ok();
            }
            else
            {
	            //Validation failed, return all errors
				foreach (var error in UserChangeEmailResult.Errors)
	            {
		            ModelState.AddModelError(string.Empty, error.Description);
		            Console.WriteLine($"Error: {error.Description}");
	            }

	            var errors = ModelState.ToDictionary(
		            kvp => kvp.Key,
		            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
	            );

	            return BadRequest(errors);
			}
           
        }

        [HttpPost]
        [Authorize]

		public async Task<IActionResult> UpdateName(ChangeNameViewModel newNameModel)
        {

	        //return BadRequest if ModelState is invalid
			if (!ModelState.IsValid)
            {
				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
				);

                return BadRequest(errors);
			}

            //Validate changing Name
			var UserChangeNameResult = await _userRepository.ChangeNameAsync(HttpContext.User, newNameModel);


			//Check validation result
			if (UserChangeNameResult.Succeeded)
            {
                return Ok();
            }
            else
            {
	            //Validation failed, return all errors
				foreach (var error in UserChangeNameResult.Errors)
	            {
		            ModelState.AddModelError(string.Empty, error.Description);
		            Console.WriteLine($"Error: {error.Description}");
	            }

	            var errors = ModelState.ToDictionary(
		            kvp => kvp.Key,
		            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
	            );

	            return BadRequest(errors);
			}
        }


		[HttpGet]
        [Authorize]

		public async Task<IActionResult> SettingsAsync()
        {
            //Get the current User
	        var CurrentUser = await _userRepository.GetCurrentUserAsync(HttpContext.User);


            //store user information in ViewBag
            ViewBag.Username = CurrentUser.UserName;
            ViewBag.Email = CurrentUser.Email;
            ViewBag.Name = CurrentUser.Name;
            ViewBag.Password = "*******";
            ViewBag.ProfilePicturePath = CurrentUser.ProfilePicturePath;



			return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CloseAccount()
        {
            //Retrieve current user
            var CurrentUser = await _userRepository.GetCurrentUserAsync(HttpContext.User);


            //logout user
            if (CurrentUser != null)
            {
                _userRepository.LogoutUserAsync();
            }

            //close account
			var UserCloseAccountResult = await _userRepository.CloseAccountAsync(CurrentUser);
            if (UserCloseAccountResult.Succeeded) { 
                return Ok();
            }
			return BadRequest(UserCloseAccountResult.Errors);
		}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeProfilePicture(ChangeProfilePictureViewModel model)
        {
	        if (!ModelState.IsValid)
	        {
		        var errors = ModelState.ToDictionary(
			        kvp => kvp.Key,
			        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
		        );

		        return BadRequest(errors);
	        }



	        
		    var (result, newFilePath) = await _userRepository.ChangeProfilePictureAsync(User, model);
            if (result.Succeeded)
            {
                return Ok(new { newProfilePictureUrl = newFilePath});
            }
            else
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Error: {error.Description}");
                }

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

                return BadRequest(errors);
            }
		}
	}
}
