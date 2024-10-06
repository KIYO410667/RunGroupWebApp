using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RunGroupWebApp.Controllers;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RunGroupWebApp.Tests.Controllers
{
    public class ClubControllerTests
    {
        private readonly Mock<IClubRepository> _mockClubRepo;
        private readonly Mock<IAzureBlobService> _mockBlobService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<ClubController>> _mockLogger;
        private readonly ClubController _controller;

        public ClubControllerTests()
        {
            _mockClubRepo = new Mock<IClubRepository>();
            _mockBlobService = new Mock<IAzureBlobService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<ClubController>>();
            _controller = new ClubController(_mockClubRepo.Object, _mockBlobService.Object, _mockHttpContextAccessor.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithPaginatedList()
        {
            // Arrange
            var clubs = new List<ClubSummaryViewModel> { new ClubSummaryViewModel(), new ClubSummaryViewModel() };
            _mockClubRepo.Setup(repo => repo.SearchClubsAsync(It.IsAny<string>(), It.IsAny<ClubCategory?>(), It.IsAny<City?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(clubs);
            _mockClubRepo.Setup(repo => repo.GetSearchResultsCountAsync(It.IsAny<string>(), It.IsAny<ClubCategory?>(), It.IsAny<City?>()))
                .ReturnsAsync(2);

            // Act
            var result = await _controller.Index(null, null, null, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<ClubSummaryViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Detail_ReturnsViewResult_WithClubWithUsersViewModel()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club" };
            var users = new List<AppUser> { new AppUser(), new AppUser() };
            _mockClubRepo.Setup(repo => repo.GetClubWithAppUserById(1))
                .ReturnsAsync(club);
            _mockClubRepo.Setup(repo => repo.GetAllUsers(1))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.Detail(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ClubWithUsersViewModel>(viewResult.Model);
            Assert.Equal(club, model.Club);
            Assert.Equal(users, model.AppUsers);
        }

        [Fact]
        public void Create_ReturnsViewResult_WithCreateClubViewModel()
        {
            // Arrange
            var userId = "testUserId";
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            var httpContext = new DefaultHttpContext() { User = claimsPrincipal };
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateClubViewModel>(viewResult.Model);
            Assert.Equal(userId, model.AppUserId);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var clubVM = new CreateClubViewModel
            {
                Title = "Test Club",
                Description = "Test Description",
                Image = Mock.Of<IFormFile>(),
                AppUserId = "testUserId",
                Address = new Address { Street = "Test Street", City = City.台北市 }
            };
            _mockBlobService.Setup(s => s.UploadPhotoAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("http://test-url.com/image.jpg");

            // Act
            var result = await _controller.Create(clubVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockClubRepo.Verify(r => r.Add(It.IsAny<Club>()), Times.Once);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithEditClubViewModel()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club", Description = "Test Description" };
            _mockClubRepo.Setup(repo => repo.GetById(1)).ReturnsAsync(club);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditClubViewModel>(viewResult.Model);
            Assert.Equal(club.Title, model.Title);
            Assert.Equal(club.Description, model.Description);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var clubVM = new EditClubViewModel
            {
                Id = 1,
                Title = "Updated Test Club",
                Description = "Updated Test Description",
                Address = new Address { Street = "Updated Test Street", City = City.台中市 }
            };
            var existingClub = new Club { Id = 1, Title = "Test Club", Description = "Test Description" };
            _mockClubRepo.Setup(repo => repo.GetById(1)).ReturnsAsync(existingClub);

            // Act
            var result = await _controller.Edit(1, clubVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockClubRepo.Verify(r => r.Update(It.IsAny<Club>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithClub()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club" };
            _mockClubRepo.Setup(repo => repo.GetById(1)).ReturnsAsync(club);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(club, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_WithExistingClub_ReturnsRedirectToActionResult()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club", Image = "http://test-url.com/image.jpg" };
            _mockClubRepo.Setup(repo => repo.GetByIdIncludeAppUserClub(1)).ReturnsAsync(club);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockBlobService.Verify(s => s.DeletePhotoByUrlAsync(It.IsAny<string>()), Times.Once);
            _mockClubRepo.Verify(r => r.Delete(It.IsAny<Club>()), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_WithDbUpdateException_ReturnsViewResult()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club" };
            _mockClubRepo.Setup(repo => repo.GetByIdIncludeAppUserClub(1)).ReturnsAsync(club);
            _mockClubRepo.Setup(repo => repo.Delete(It.IsAny<Club>())).Throws(new DbUpdateException());

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(club, viewResult.Model);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(string.Empty));
        }
    }
}
