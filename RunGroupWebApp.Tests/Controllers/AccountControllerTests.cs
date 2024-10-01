using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RunGroupWebApp.Controllers;
using RunGroupWebApp.Data;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Security.Claims;

namespace RunGroupWebApp.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUserManager = MockUserManager<AppUser>();
            _mockSignInManager = MockSignInManager();
            _controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task Login_Get_ReturnsViewWithLoginViewModel()
        {
            // Act
            var result = await _controller.Login() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoginViewModel>(result.Model);
        }

        [Fact]
        public async Task Login_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var loginVM = new LoginViewModel();
            _controller.ModelState.AddModelError("Error", "Model error");

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.Equal(loginVM, viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_UserNotFound_ReturnsViewWithError()
        {
            // Arrange
            var loginVM = new LoginViewModel { Email = "test@example.com", Password = "password" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.Equal(loginVM, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("Email"));
        }

        [Fact]
        public async Task Login_Post_InvalidPassword_ReturnsViewWithError()
        {
            // Arrange
            var loginVM = new LoginViewModel { Email = "test@example.com", Password = "password" };
            var user = new AppUser { Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.Equal(loginVM, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("Password"));
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToHome()
        {
            // Arrange
            var loginVM = new LoginViewModel { Email = "test@example.com", Password = "password" };
            var user = new AppUser { Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Login(loginVM) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public void Register_Get_ReturnsViewWithRegisterViewModel()
        {
            // Act
            var result = _controller.Register() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RegisterViewModel>(result.Model);
        }

        [Fact]
        public async Task Register_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var registerVM = new RegisterViewModel();
            _controller.ModelState.AddModelError("Error", "Model error");

            // Act
            var result = await _controller.Register(registerVM) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RegisterViewModel>(result.Model);
        }

        [Fact]
        public async Task Register_Post_ExistingEmail_ReturnsViewWithError()
        {
            // Arrange
            var registerVM = new RegisterViewModel { Email = "test@example.com", Password = "password" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());

            // Act
            var result = await _controller.Register(registerVM) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RegisterViewModel>(result.Model);
            Assert.True(_controller.ModelState.ContainsKey("Email"));
        }

        [Fact]
        public async Task Register_Post_SuccessfulRegistration_RedirectsToHome()
        {
            // Arrange
            var registerVM = new RegisterViewModel { Email = "test@example.com", Password = "password" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerVM) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public async Task Logout_RedirectsToHome()
        {
            // Act
            var result = await _controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<AppUser>> MockSignInManager()
        {
            return new Mock<SignInManager<AppUser>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
                null, null, null, null);
        }
    }
}