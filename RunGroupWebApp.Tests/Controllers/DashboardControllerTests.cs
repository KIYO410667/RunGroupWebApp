using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RunGroupWebApp.Controllers;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using Xunit;

namespace RunGroupWebApp.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardRepository> _mockRepository;
        private readonly Mock<IAzureBlobService> _mockBlobService;
        private readonly Mock<ILogger<DashboardController>> _mockLogger;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _mockRepository = new Mock<IDashboardRepository>();
            _mockBlobService = new Mock<IAzureBlobService>();
            _mockLogger = new Mock<ILogger<DashboardController>>();
            _controller = new DashboardController(_mockRepository.Object, _mockBlobService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithDashboardViewModel()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllUserClub()).ReturnsAsync(new List<Club>());
            _mockRepository.Setup(repo => repo.GetUserById()).ReturnsAsync(new AppUser());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DashboardViewModel>(viewResult.Model);
            Assert.NotNull(model.Clubs);
            Assert.NotNull(model.appUser);
        }

        [Fact]
        public async Task EditUserProfile_Get_ReturnsViewResult_WithEditUserProfileViewModel()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "1",
                UserName = "TestUser",
                Bio = "Test Bio",
                ProfilePhotoUrl = "http://test.com/photo.jpg",
                Address = new Address { City = Data.Enum.City.台北市, Street = "TestStreet" }
            };
            _mockRepository.Setup(repo => repo.GetUserById()).ReturnsAsync(user);

            // Act
            var result = await _controller.EditUserProfile();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditUserProfileViewModel>(viewResult.Model);
            Assert.Equal(user.Id, model.Id);
            Assert.Equal(user.UserName, model.UserName);
            Assert.Equal(user.Bio, model.Bio);
            Assert.Equal(user.ProfilePhotoUrl, model.ProfilePhotoUrl);
            Assert.Equal(user.Address.City, model.Address.City);
            Assert.Equal(user.Address.Street, model.Address.Street);
        }

        [Fact]
        public async Task EditUserProfile_Get_ReturnsErrorView_WhenUserNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserById()).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.EditUserProfile();

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task EditUserProfile_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var editUserVM = new EditUserProfileViewModel
            {
                Id = "1",
                UserName = "UpdatedUser",
                Bio = "Updated Bio",
                Address = new Address { City = Data.Enum.City.台北市, Street = "UpdatedStreet" }
            };
            var currentUser = new AppUser
            {
                Id = "1",
                UserName = "TestUser",
                Bio = "Test Bio",
                Address = new Address { City = Data.Enum.City.台中市, Street = "TestStreet" }
            };
            _mockRepository.Setup(repo => repo.GetUserById()).ReturnsAsync(currentUser);

            // Act
            var result = await _controller.EditUserProfile(editUserVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<AppUser>()), Times.Once);
        }

        [Fact]
        public async Task EditUserProfile_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var editUserVM = new EditUserProfileViewModel();
            _controller.ModelState.AddModelError("Error", "Model State is invalid");

            // Act
            var result = await _controller.EditUserProfile(editUserVM);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<EditUserProfileViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task EditUserProfile_Post_UploadsNewPhoto_WhenProfilePhotoFileIsProvided()
        {
            // Arrange
            var editUserVM = new EditUserProfileViewModel
            {
                Id = "1",
                UserName = "TestUser",
                ProfilePhotoFile = new Mock<IFormFile>().Object,
                Address = new Address()
            };
            var currentUser = new AppUser { Id = "1", UserName = "TestUser" };
            _mockRepository.Setup(repo => repo.GetUserById()).ReturnsAsync(currentUser);
            _mockBlobService.Setup(blob => blob.UploadPhotoAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                            .ReturnsAsync("http://test.com/newphoto.jpg");

            // Act
            var result = await _controller.EditUserProfile(editUserVM);

            // Assert
            _mockBlobService.Verify(blob => blob.UploadPhotoAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
            Assert.Equal("http://test.com/newphoto.jpg", currentUser.ProfilePhotoUrl);
        }
    }
}
