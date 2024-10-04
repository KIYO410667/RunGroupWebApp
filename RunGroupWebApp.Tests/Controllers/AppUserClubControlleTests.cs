using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RunGroupWebApp.Controllers;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RunGroupWebApp.Tests.Controllers
{
    public class AppUserClubControllerTests
    {
        private readonly Mock<IAppUserClubRepository> _mockRepo;
        private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        private readonly Mock<ILogger<AppUserClubController>> _mockLogger;
        private readonly AppUserClubController _controller;

        public AppUserClubControllerTests()
        {
            _mockRepo = new Mock<IAppUserClubRepository>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<AppUserClubController>>();
            _controller = new AppUserClubController(_mockRepo.Object, _mockContextAccessor.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Add_NewClub_ReturnsRedirectToActionResult()
        {
            // Arrange
            int clubId = 1;
            string userId = "testUser";
            SetupHttpContext(userId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(clubId)).ReturnsAsync((AppUserClub)null);

            // Act
            var result = await _controller.Add(clubId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("AppUserClub", redirectResult.ControllerName);
            _mockRepo.Verify(repo => repo.Add(It.IsAny<AppUserClub>()), Times.Once);
            VerifyLog(LogLevel.Information, $"User {userId} added to club {clubId}");
        }

        [Fact]
        public async Task Add_ExistingClub_ReturnsRedirectToActionResult()
        {
            // Arrange
            int clubId = 1;
            string userId = "testUser";
            SetupHttpContext(userId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(clubId)).ReturnsAsync(new AppUserClub());

            // Act
            var result = await _controller.Add(clubId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Club", redirectResult.ControllerName);
            _mockRepo.Verify(repo => repo.Add(It.IsAny<AppUserClub>()), Times.Never);
            VerifyLog(LogLevel.Information, $"User {userId} already in club {clubId}. Redirecting to Club Index.");
        }

        [Fact]
        public async Task Add_ThrowsException_ReturnsErrorView()
        {
            // Arrange
            int clubId = 1;
            SetupHttpContext("testUser");
            _mockRepo.Setup(repo => repo.GetByIdAsync(clubId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Add(clubId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            VerifyLog(LogLevel.Error, $"An error occurred while adding user to club with ID: {clubId}");
        }

        [Fact]
        public async Task Delete_ExistingClub_ReturnsRedirectToActionResult()
        {
            // Arrange
            int clubId = 1;
            _mockRepo.Setup(repo => repo.GetByIdAsync(clubId)).ReturnsAsync(new AppUserClub());

            // Act
            var result = await _controller.Delete(clubId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<AppUserClub>()), Times.Once);
            VerifyLog(LogLevel.Information, $"User club with ID {clubId} deleted successfully");
        }

        [Fact]
        public async Task Delete_NonExistentClub_ReturnsErrorView()
        {
            // Arrange
            int clubId = 1;
            _mockRepo.Setup(repo => repo.GetByIdAsync(clubId)).ReturnsAsync((AppUserClub)null);

            // Act
            var result = await _controller.Delete(clubId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<AppUserClub>()), Times.Never);
            VerifyLog(LogLevel.Warning, $"User club with ID {clubId} not found for deletion");
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfClubs()
        {
            // Arrange
            var expectedClubs = new List<Club> { new Club(), new Club() };
            _mockRepo.Setup(repo => repo.GetAllClubs()).ReturnsAsync(expectedClubs);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Club>>(viewResult.Model);
            Assert.Equal(expectedClubs.Count, model.Count());
            VerifyLog(LogLevel.Information, $"Retrieved {expectedClubs.Count} clubs for user");
        }

        private void SetupHttpContext(string userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            _mockContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        }

        private void VerifyLog(LogLevel logLevel, string message)
        {
            _mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}
