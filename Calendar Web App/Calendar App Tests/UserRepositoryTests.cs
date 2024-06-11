using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.Repositories;
using Calendar_Web_App.ViewModels.AccountAccessViewModels;
using Calendar_Web_App.ViewModels.AccountSettingsViewModels;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Calendar_App_Tests
{
	public class UserRepositoryTests
	{
		private readonly Mock<UserManager<User>> _mockUserManager;
		private readonly Mock<SignInManager<User>> _mockSignInManager;
		private readonly Mock<ILogger<UserRepository>> _mockLogger;
		private readonly Mock<IApplicationDbContext> _context;
		private readonly UserRepository _userRepository;


		public UserRepositoryTests()
		{
			var userStoreMock = new Mock<IUserStore<User>>();
			_mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<IPasswordHasher<User>>().Object,
				new IUserValidator<User>[0],
				new IPasswordValidator<User>[0],
				new Mock<ILookupNormalizer>().Object,
				new Mock<IdentityErrorDescriber>().Object,
				new Mock<IServiceProvider>().Object,
				new Mock<ILogger<UserManager<User>>>().Object);
			_mockSignInManager = new Mock<SignInManager<User>>(
				_mockUserManager.Object,
				new Mock<IHttpContextAccessor>().Object,
				new Mock<IUserClaimsPrincipalFactory<User>>().Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<ILogger<SignInManager<User>>>().Object,
				new Mock<IAuthenticationSchemeProvider>().Object,
				new Mock<IUserConfirmation<User>>().Object);

			_mockLogger = new Mock<ILogger<UserRepository>>();
			_context = new Mock<IApplicationDbContext>();
			_userRepository = new UserRepository(_mockUserManager.Object, _mockSignInManager.Object, _mockLogger.Object, _context.Object);
		}


		[Theory]
		[InlineData("user1", true)]
		[InlineData("user2", false)]
		public async Task UserRepository_GetCurrentUserAsync_ReturnsUserIfExistsElseReturnsNull(string userName, bool ReturnsUser)
		{
			//Arrange
			var user = ReturnsUser ? new User { Id = "1", Name = userName, UserName = "TestUser", Email = "TestEmail@email.com" } : null;

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, userName),
			}));

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			//Act
			var result = await _userRepository.GetCurrentUserAsync(claimsPrincipal);

			if (ReturnsUser)
			{
				Assert.Equal(userName, result.Name);
				Assert.Equal("1", result.Id);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"User {userName} with Id {user.Id} was successfully found")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
			else
			{
				Assert.Null(result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"User {userName} does not exist")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
		}


		[Fact]
		public async Task UserRepository_GetCurrentUserAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testuser")
			}));


			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ThrowsAsync(new InvalidOperationException("Test exception"));

			//Act
			var result = await _userRepository.GetCurrentUserAsync(claimsPrincipal);

			//Assert
			Assert.Equal(result, null);

			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to retrieve user from database")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}




		[Theory]
		[InlineData("Login1","Password1", true)]
		[InlineData("Login2","Password2", false)]
		public async Task UserRepository_LoginUserAsync_ReturnsPositiveResultIfUserExistsElseNegativeResult(string login, string password, bool LoginSucceeds)
		{
			//Arrange
			var loginModel = new LoginViewModel {
				Login = login,
				Password = password,
			};

			_mockSignInManager.Setup(sim => sim.PasswordSignInAsync(loginModel.Login, loginModel.Password, true, false)).ReturnsAsync(LoginSucceeds ? SignInResult.Success : SignInResult.Failed);


			//Act
			var result = await _userRepository.LoginUserAsync(loginModel);

			//Assert
			if (LoginSucceeds)
			{
				Assert.True(result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully logged in")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
			else
			{
				Assert.False(result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User could not be logged in")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
		}

		[Fact]
		public async Task UserRepository_LoginUserAsync_ThrowsInvalidOperationException_ReturnsSignInResultFailed()
		{
			//Arrange
			var loginModel = new LoginViewModel
			{
				Login = "login",
				Password = "password",
			};

			_mockSignInManager.Setup(sim => sim.PasswordSignInAsync(loginModel.Login, loginModel.Password, true, false)).ThrowsAsync(new InvalidOperationException("An InvalidOperationException occured while trying to Log In"));

			//Act
			var result = await _userRepository.LoginUserAsync(loginModel);

			//Assert

			Assert.Equal(SignInResult.Failed, result);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Log In")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}


		[Theory]
		[InlineData("password1", false)]
		[InlineData("password2", true)]
		public async Task UserRepository_RegisterUserAsync_ReturnsPositiveResultIfCreatedUserElseNegativeResult(string userPassword, bool RegisterSuccessful)
		{
			//Arrange
			var UserToRegister = RegisterSuccessful ? new RegisterViewModel
			{Name = "user1",
			 Email = "testemail@gmail.com",
			 Login = "testUsername", 
			 Password = userPassword 
			} : (RegisterViewModel)null;

			var User = UserToRegister != null ? new User {
				Name = UserToRegister.Name,
				Email = UserToRegister.Email,
				UserName = UserToRegister.Login
			} : (User)null;

			if (UserToRegister != null)
			{
				_mockUserManager.Setup(um => um.CreateAsync(It.Is<User>(u => u.UserName == UserToRegister.Login && u.Email == UserToRegister.Email), UserToRegister.Password)).ReturnsAsync(RegisterSuccessful ? IdentityResult.Success : IdentityResult.Failed());
			}

			//Act

			var result = await _userRepository.RegisterUserAsync(UserToRegister);


			//Assert
			if(UserToRegister == null)
			{
				
				_mockLogger.Verify(x => x.Log(
					LogLevel.Warning,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("RegisterModel is null")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
				return;
			}

			if (RegisterSuccessful)
			{
				Assert.True(result.Succeeded);
				_mockLogger.Verify(x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Registering User was successful")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
			else
			{
				Assert.False(result.Succeeded);
				_mockLogger.Verify(x => x.Log(
					LogLevel.Warning,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User could not be registered")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
		}


		[Fact]
		public async Task UserRepository_RegisterUserAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			var password = "testPassword";
			var RegisterModel = new RegisterViewModel { Email = "testEmail@gmail.com", Login = "testLogin", Name = "testName", Password = password};
			var User = new User { Name = RegisterModel.Name, Email = RegisterModel.Email, UserName = RegisterModel.Login };

			_mockUserManager.Setup(um => um.CreateAsync(It.Is<User>(u => 
			u.UserName == RegisterModel.Login &&
			u.Email == RegisterModel.Email &&
			u.Name == RegisterModel.Name), password)).ThrowsAsync(new InvalidOperationException("An InvalidOperationException occured while trying to Register"));

			//Act + Assert
			var result = await _userRepository.RegisterUserAsync(RegisterModel);

			//Assert
			Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);

			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Register")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

		}

		[Theory]
		[InlineData("TestPassword1!", "TestPassword2!", true)]
		[InlineData("TestPassword1!", "", false)]
		public async Task UserRepository_ChangePasswordAsync_ReturnsPositiveResultIfPasswordChangedElseNegativeResult(string oldPassword, string newPassword, bool passwordChangedSuccessfully)
		{
			//Arrange
			ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testuser")
			}));

			User user = new User
			{
				Email = "testemail@gmail.com",
				Name = "testname",
				UserName = "testUsername"
			};

			ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel
			{
				OldPassword = oldPassword,
				NewPassword = newPassword,
				ConfirmPassword = newPassword
			};

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);
			_mockUserManager.Setup(um => um.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword)).
				ReturnsAsync(passwordChangedSuccessfully ? IdentityResult.Success : IdentityResult.Failed());

			//Act
			var result = await _userRepository.ChangePasswordAsync(claimsPrincipal, changePasswordViewModel);

			//Assert
			if (passwordChangedSuccessfully)
			{
				Assert.Equal(IdentityResult.Success, result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Changing User {user.Name} password was successful")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
				_mockUserManager.Verify(um => um.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword), Times.Once);

			}
			else
			{
				Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Changing User {user.Name} password has failed")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
				_mockUserManager.Verify(um => um.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword), Times.Once);
			}


		}

		[Fact]
		public async Task UserRepository_ChangePasswordAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testuser")
			}));

			User user = new User
			{
				Email = "testemail@gmail.com",
				Name = "testname",
				UserName = "testUsername"
			};

			ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel
			{
				OldPassword = "oldPassword",
				NewPassword = "newPassword",
				ConfirmPassword = "newPassword"
			};

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);
			_mockUserManager.Setup(um => um.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword)).
				ThrowsAsync(new InvalidOperationException("Invalid operation exception message"));

			//Act
			var result = await _userRepository.ChangePasswordAsync(claimsPrincipal, changePasswordViewModel);

			//Assert
			Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Change Password")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_mockUserManager.Verify(um => um.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword), Times.Once);

		}


		[Fact]
		public async Task UserRepository_LogoutUserAsync_CallsSignOut()
		{
			//Arrange
			_mockSignInManager.Setup(sim => sim.SignOutAsync()).Returns(Task.CompletedTask);
			
			
			//Act
			_userRepository.LogoutUserAsync();
			
			
			//Assert
			_mockSignInManager.Verify(sim => sim.SignOutAsync(), Times.Once);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully logged out")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}


		[Fact]
		public async Task UserRepository_LogoutUserAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			_mockSignInManager.Setup(sim => sim.SignOutAsync()).ThrowsAsync(new InvalidOperationException("Invalid operation exception"));


			//Act + Assert
			_userRepository.LogoutUserAsync();

			//Assert
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Change Password")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_mockSignInManager.Verify(um => um.SignOutAsync(), Times.Once);
			await Assert.ThrowsAsync<InvalidOperationException>(() => _userRepository.LogoutUserAsync());
		}


		[Theory]
		[InlineData("Username1", true)]
		[InlineData("Username2", false)]
		public async Task UserRepository_CahngeUsernameAsync_ReturnsPositiveResultIfUsernameChangedElseNegativeResult(string userName, bool changedUsernameSuccessfully)
		{
			//Setup
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, userName),
			}));

			var newUsernameModel = new ChangeUsernameViewModel
			{
				Username = userName
			};

			User user = changedUsernameSuccessfully ? new User
			{
				Email = "testemail@gmail.com",
				Name = "testname",
				UserName = userName
			} : null;

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			_mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(changedUsernameSuccessfully ? IdentityResult.Success : IdentityResult.Failed());


			//Act
			var result = await _userRepository.ChangeUsernameAsync(claimsPrincipal, newUsernameModel);

			//Assert
			if (changedUsernameSuccessfully)
			{
				Assert.Equal(IdentityResult.Success, result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updating Username was successful")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());

				_mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);

			}
			else
			{
				Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updating Username has failed")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());

			}

		}


		[Fact]
		public async Task UserRepository_ChangeUsernameAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testUsername"),
			}));

			var newUsernameModel = new ChangeUsernameViewModel
			{
				Username = "testUsername"
			};

			User user = new User
			{
				Email = "testemail@gmail.com",
				Name = "testname",
				UserName = "testUsername"
			};

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			_mockUserManager.Setup(um => um.UpdateAsync(user)).ThrowsAsync(new InvalidOperationException("invalid operation exception"));

			//Act
			await _userRepository.ChangeUsernameAsync(claimsPrincipal, newUsernameModel);

			//Assert
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Change Username")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);

		}


		[Theory]
		[InlineData("testEmail@gmail.com", true)]
		[InlineData("testEmail2@gmail.com", false)]
		public async Task UserRepository_ChangeEmailAsync_ReturnsPositiveResultIfEmailChangedElseNegativeResult(string email, bool changedEmailSuccessfully)
		{
			//Setup
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testName"),
			}));

			var newEmailModel = new ChangeEmailViewModel
			{
				Email = email
			};

			User user = changedEmailSuccessfully ? new User
			{
				Email = email,
				Name = "testname",
				UserName = "testUsername"
			} : null;

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			_mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(changedEmailSuccessfully ? IdentityResult.Success : IdentityResult.Failed());


			//Act
			var result = await _userRepository.ChangeEmailAsync(claimsPrincipal, newEmailModel);

			//Assert
			if (changedEmailSuccessfully)
			{
				Assert.Equal(IdentityResult.Success, result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Successfully updated {user.Name}'s email")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
				_mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);

			}
			else
			{
				Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Changing email has failed")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
			}

		}
		[Fact]
		public async Task UserRepository_ChangeEmailAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testName"),
			}));

			var newEmailModel = new ChangeEmailViewModel
			{
				Email = "testEmail"
			};

			User user = new User
			{
				Email = "testEmail",
				Name = "testname",
				UserName = "testUsername"
			};


		
			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			_mockUserManager.Setup(um => um.UpdateAsync(user)).ThrowsAsync(new InvalidOperationException("invalid operation exception"));

			//Act
			await _userRepository.ChangeEmailAsync(claimsPrincipal, newEmailModel);

			//Assert
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Change Email")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);

		}

		[Theory]
		[InlineData("testName", true)]
		[InlineData("testName2", false)]
		public async Task UserRepository_ChangeNameAsync_ReturnsPositiveResultIfNameChangedElseNegativeResult(string name, bool changedNameSuccessfully)
		{
			//Setup
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testName"),
			}));

			var newNameModel = new ChangeNameViewModel
			{
				Name = name
			};

			User user = changedNameSuccessfully ? new User
			{
				Email = "testEmail@gmail.com",
				Name = name,
				UserName = "testUsername"
			} : null;

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			_mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(changedNameSuccessfully ? IdentityResult.Success : IdentityResult.Failed());


			//Act
			var result = await _userRepository.ChangeNameAsync(claimsPrincipal, newNameModel);

			//Assert
			if (changedNameSuccessfully)
			{
				Assert.Equal(IdentityResult.Success, result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Changing User's Name was successful")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());

				_mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);

			}
			else
			{
				Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Changing User's Name has failed")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
			}

		}

		[Fact]
		public async Task UserRepository_ChangeNameAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "testName"),
			}));

			var newNameModel = new ChangeNameViewModel
			{
				Name = "testName"
			};

			User user = new User
			{
				Email = "testEmail@gmail.com",
				Name = "testName",
				UserName = "testUsername"
			};

			_mockUserManager.Setup(um => um.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

			_mockUserManager.Setup(um => um.UpdateAsync(user)).ThrowsAsync(new InvalidOperationException("invalid operation exception"));

			//Act
			await _userRepository.ChangeNameAsync(claimsPrincipal, newNameModel);

			//Assert
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Change Name")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);

		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task UserRepository_CloseAccountAsync_ClosesAccountIfSuccessfulElseFails(bool closedAccountSuccessfully)
		{
			//Arrange
			User user = closedAccountSuccessfully ? new User
			{
				Email = "testEmail",
				Name = "testName",
				UserName = "testUsername",
			} : null;

			_mockUserManager.Setup(um => um.DeleteAsync(user)).ReturnsAsync(closedAccountSuccessfully ? IdentityResult.Success : IdentityResult.Failed());

			//Act
			var result = await _userRepository.CloseAccountAsync(user);


			//Assert
			if (closedAccountSuccessfully)
			{
				Assert.Equal(IdentityResult.Success, result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Closing account was successful")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());

				_mockUserManager.Verify(um => um.DeleteAsync(user), Times.Once);

			}
			else
			{
				Assert.Equal(IdentityResult.Failed().Succeeded, result.Succeeded);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Closing account has failed")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
			}
		}

		[Fact]
		public async Task UserRepository_CloseAccountAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			User user = new User
			{
				Email = "testEmail",
				Name = "testName",
				UserName = "testUsername",
			};

			_mockUserManager.Setup(um => um.DeleteAsync(user)).ThrowsAsync(new InvalidOperationException("invalid operation exception"));

			//Act
			var result = _userRepository.CloseAccountAsync(user);

			//Assert
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Close Account")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_mockUserManager.Verify(um => um.DeleteAsync(user), Times.Once);

		}


	}
}
