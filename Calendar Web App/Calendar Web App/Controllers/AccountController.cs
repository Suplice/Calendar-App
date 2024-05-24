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
		public async Task<IActionResult> Login(LoginViewModel user)
        {
	        
			if (!ModelState.IsValid)
            {
                //Model invalid - action returns login view with old model as parameter
                return View(user);
            }


         
            //validating user data
            var validate = await _userRepository.LoginUserAsync(user);


            //data is correct, log in and proceed to Calendar view
            if (validate.Succeeded)
            {
	            return RedirectToAction("Calendar", "Calendar");
			}


			//Account is on lockout
			if (validate.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account is on Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt, Check your login or password");
            }


			//validation is incorrect, return login view with odl model
			return View(user);
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
		public async Task<IActionResult> Register(RegisterViewModel newUser)
        {
            //Checking whether model state is valid
            if (!ModelState.IsValid)
            {
                //Model state is invalid - action returns to Register view with old model as parameter
                return View(newUser);
            }

            
            //validating User data
            var validate = await _userRepository.RegisterUserAsync(newUser);



            if (validate.Succeeded)
            {
                //validation succeeded - redirect to Login action
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //Validation is incorrect - add all errors
                foreach (var error in validate.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Error: {error.Description}");
                }


                //return to register view with old model as parameter
                return View(newUser);
            }



        }


        [Authorize]
        [HttpPost]
		public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel PasswordModel)
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
            var validate = await _userRepository.ChangePasswordAsync(HttpContext.User, PasswordModel);

            if (validate.Succeeded)
            {
                return Ok();
            }
            else
			{


				//Validation failed, return all errors
				foreach (var error in validate.Errors)
				{
                    if (error.Description == "Incorrect password.")
                    {
                        ModelState.AddModelError("OldPassword", error.Description);
                    }
                    else {
                        ModelState.AddModelError(string.Empty, error.Description);
                        Console.WriteLine($"Error: {error.Description}");
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
		public async Task<IActionResult> UpdateUsername(ChangeUsernameViewModel newUsername)
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
            var validate = await _userRepository.ChangeUsernameAsync(HttpContext.User, newUsername);


            //Check validation result
            if (validate.Succeeded)
            {
                return Ok();
            }
            else
            {
                //Validation failed, return all errors
                foreach (var error in validate.Errors)
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

		public async Task<IActionResult> UpdateEmail(ChangeEmailViewModel newEmail)
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
			var validate = await _userRepository.ChangeEmailAsync(HttpContext.User, newEmail);


			//Check validation result
			if (validate.Succeeded)
            {
                return Ok();
            }
            else
            {
	            //Validation failed, return all errors
				foreach (var error in validate.Errors)
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

		public async Task<IActionResult> UpdateName(ChangeNameViewModel newName)
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
			var validate = await _userRepository.ChangeNameAsync(HttpContext.User, newName);


			//Check validation result
			if (validate.Succeeded)
            {
                return Ok();
            }
            else
            {
	            //Validation failed, return all errors
				foreach (var error in validate.Errors)
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
	        var user = await _userRepository.GetCurrentUserAsync(HttpContext.User);


            //store user information in ViewBag
            ViewBag.Username = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.Name = user.Name;
            ViewBag.Password = "*******";


            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CloseAccount()
        {
            //Retrieve current user
            var user = await _userRepository.GetCurrentUserAsync(HttpContext.User);


            //logout user
            if (user != null)
            {
                _userRepository.LogoutUserAsync();
            }

            //close account
			var validation = await _userRepository.CloseAccountAsync(user);
            if (validation.Succeeded) { 
                return Ok();
            }
			return BadRequest(validation.Errors);
		}
    }
}
