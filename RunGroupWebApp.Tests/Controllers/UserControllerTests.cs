using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfUsers()
        {
            // Arrange
            var expectedUserVM = new List<UserViewModel> { new UserViewModel(), new UserViewModel() };
            _mockRepo.Setup(repo => repo.GetAllUser())
                .ReturnsAsync(expectedUserVM);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(viewResult.Model);
            Assert.Equal(expectedUserVM.Count, model.Count());
        }

        [Fact]
        public async Task Index_LogsCorrectMessages()
        {
            // Arrange
            var expectedUserVM = new List<UserViewModel> { new UserViewModel(), new UserViewModel() };
            _mockRepo.Setup(repo => repo.GetAllUser())
                .ReturnsAsync(expectedUserVM);

            // Act
            await _controller.Index();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Fetching all users")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Retrieved {expectedUserVM.Count} users")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Index_CatchesException_ReturnsErrorView()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllUser())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("An error occurred while fetching all users")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Detail_ReturnsAViewResult_WithUserSummary()
        {
            // Arrange
            var userId = "testId";
            var expectedUser = new UserViewModel { Id = userId };
            _mockRepo.Setup(repo => repo.GetUserSummaryById(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.Detail(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(userId, model.Id);
        }

        [Fact]
        public async Task Detail_WithNonExistentUser_ReturnsErrorView()
        {
            // Arrange
            var userId = "nonExistentId";
            _mockRepo.Setup(repo => repo.GetUserSummaryById(userId))
                .ReturnsAsync((UserViewModel)null);

            // Act
            var result = await _controller.Detail(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"User with ID {userId} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Detail_CatchesException_ReturnsErrorView()
        {
            // Arrange
            var userId = "testId";
            _mockRepo.Setup(repo => repo.GetUserSummaryById(userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Detail(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"An error occurred while fetching details for user {userId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}
