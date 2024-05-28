using Calendar_Web_App.Models;
using Calendar_Web_App.Repositories;
using Calendar_Web_App.ViewModels.AccountAccessViewModels;
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
		private readonly UserRepository _userRepository;

		public UserRepositoryTests()
		{
			var userStoreMock = new Mock<IUserStore<User>>();
			_mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
			_mockSignInManager = new Mock<SignInManager<User>>(
				_mockUserManager.Object,
				new Mock<IHttpContextAccessor>().Object,
				new Mock<IUserClaimsPrincipalFactory<User>>().Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<ILogger<SignInManager<User>>>().Object,
				new Mock<IAuthenticationSchemeProvider>().Object,
				new Mock<IUserConfirmation<User>>().Object);

			_mockLogger = new Mock<ILogger<UserRepository>>();
			_userRepository = new UserRepository(_mockUserManager.Object, _mockSignInManager.Object, _mockLogger.Object);
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

			//Act + Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => _userRepository.GetCurrentUserAsync(claimsPrincipal));

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
		public async Task UserRepository_LoginUserAsync_ThrowsInvalidOperationException()
		{
			//Arrange
			var loginModel = new LoginViewModel
			{
				Login = "login",
				Password = "password",
			};

			_mockSignInManager.Setup(sim => sim.PasswordSignInAsync(loginModel.Login, loginModel.Password, true, false)).ThrowsAsync(new InvalidOperationException("An InvalidOperationException occured while trying to Log In"));

			//Act + Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => _userRepository.LoginUserAsync(loginModel));
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
			await Assert.ThrowsAsync<InvalidOperationException>(() => _userRepository.RegisterUserAsync(RegisterModel));
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occured while trying to Register")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

		}
	}
}
