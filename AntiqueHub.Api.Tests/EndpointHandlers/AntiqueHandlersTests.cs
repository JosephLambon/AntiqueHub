using AutoMapper;
using AntiqueHub.Core.Models;
using AntiqueHub.Api.Profiles;
using AntiqueHub.Api.Tests.Mocks;
using AntiqueHub.Api.Tests.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using AntiqueHub.Api.EndpointsHandlers;
using AntiqueHub.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace AntiqueHub.Api.Tests.EndpointHandlers
{
    public class GetAntiquesAsyncTests
    {
        private readonly ILoggerFactory mockLoggerFactory;
        private readonly IMapper mockMapper;
        private readonly Mock<ILogger<Antique>> mockLogger;
        private readonly Mock<IFileService> mockFileService;

        public GetAntiquesAsyncTests()
        {
            mockLoggerFactory = NullLoggerFactory.Instance;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AntiqueProfile());
            }, mockLoggerFactory);
            mockMapper = config.CreateMapper();
            mockLogger = new Mock<ILogger<Antique>>();
            mockFileService = new Mock<IFileService>();
            
            mockFileService
                .Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string[]>()))
                .ReturnsAsync("mocked-image.png");
        }

        [Fact]
        public async Task GetAntiquesAsync_ReturnsAvailableAntiquesByDefault()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
        
            // Act
            var result = await AntiqueHandlers.GetAntiquesAsync(
                mockRepo.Object,
                mockMapper,
                mockLogger.Object,
                includeAvailable: true, includeSold: false, includeArchived: false);
        
            // Assert
            var okResult = Assert.IsType<Ok<IEnumerable<AntiqueForResponseDto>>>(result);
            var antiques = okResult.Value.ToList();
        
            Assert.NotEmpty(antiques);
            Assert.All(antiques, a => Assert.Equal(Status.Available, a.Status));
        }
        
        [Fact]
        public async Task GetAntiquesAsync_ReturnsSoldAntiques_WhenIncludeSoldIsTrue()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
        
            // Act
            var result = await AntiqueHandlers.GetAntiquesAsync(
                mockRepo.Object,
                mockMapper,
                mockLogger.Object,
                includeAvailable: false, includeSold: true, includeArchived: false);
        
            // Assert
            var okResult = Assert.IsType<Ok<IEnumerable<AntiqueForResponseDto>>>(result);
            var antiques = okResult.Value.ToList();
        
            Assert.Single(antiques); // only 1 mock antique is Sold
            Assert.Equal(Status.Sold, antiques[0].Status);
        }
        
        [Fact]
        public async Task GetAntiquesAsync_ReturnsAllStatuses_WhenAllFlagsTrue()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
        
            // Act
            var result = await AntiqueHandlers.GetAntiquesAsync(
                mockRepo.Object,
                mockMapper,
                mockLogger.Object,
                includeAvailable: true, includeSold: true, includeArchived: true);
        
            // Assert
            var okResult = Assert.IsType<Ok<IEnumerable<AntiqueForResponseDto>>>(result);
            var antiques = okResult.Value.ToList();
        
            Assert.Equal(3, antiques.Count);
            Assert.Contains(antiques, a => a.Status == Status.Available);
            Assert.Contains(antiques, a => a.Status == Status.Sold);
            Assert.Contains(antiques, a => a.Status == Status.Archived);
        }
        
        [Fact]
        public async Task GetAntiqueByIdAsync_ReturnsOk_WhenAntiqueExists()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var expectedAntique = new AntiqueForResponseDto()
            {
                Name = "Victorian Chair",
                Description = "A 19th century Victorian-style chair made of oak.",
                Status = Status.Available,
                Price = 250.00m,
                Thumbnail = "3f8b1e1c-6c4f-4a4d-9e1f-8c3d82a1f5e4.jpg",
                Images = new[]
                {
                    "3f8b1e1c-6c4f-4a4d-9e1f-8c3d82a1f5e4.jpg",
                    "9c6c1c92-8c88-41d9-b13d-672e9a114c5d.png"
                }
            };
            
            // Act
            var result = await AntiqueHandlers.GetAntiqueByIdAsync(
                mockRepo.Object,
                mockMapper,
                1, // existing ID
                mockLogger.Object);

            // Assert
            var okResult = Assert.IsType<Ok<AntiqueForResponseDto>>(result.Result);
            Assert.Equivalent(expectedAntique, okResult.Value);
        }

        [Fact]
        public async Task GetAntiqueByIdAsync_ReturnsNotFound_WhenAntiqueDoesNotExist()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();

            // Act
            var result = await AntiqueHandlers.GetAntiqueByIdAsync(
                mockRepo.Object,
                mockMapper,
                999, // non-existent ID
                mockLogger.Object);

            // Assert
            var notFoundResult = Assert.IsType<NotFound<string>>(result.Result);
            Assert.Equal("Unable to retrieve antique with ID: 999", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateAntiqueAsync_ReturnsCreatedAtRoute_WhenValid()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var dto = new AntiqueForCreationDto
            {
                Name = "Test Antique",
                Description = "A test antique",
                Price = 50.0m,
                Status = Status.Available,
                ImageFiles = new FormFileCollection
                {
                    FormFileHelpers.CreateFormFile("mocked-image.png")
                }
            };
            var expectedResponse = new AntiqueForResponseDto
            {
                Name = "Test Antique",
                Description = "A test antique",
                Price = 50.0m,
                Status = Status.Available,
                Images = new[] { "mocked-image.png" },
                Thumbnail = "mocked-image.png"
            };

            // Act
            var result = await AntiqueHandlers.CreateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                dto,
                mockLogger.Object,
                mockFileService.Object
            );

            // Assert
            var createdResult = Assert.IsType<CreatedAtRoute<AntiqueForResponseDto>>(result.Result);
            Assert.Equivalent(expectedResponse, createdResult.Value);

            mockRepo.Verify(r => r.AddAntiqueAsync(It.IsAny<Antique>()), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAntiqueAsync_ReturnsBadRequest_WhenNoImageFiles()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var dto = new AntiqueForCreationDto
            {
                Name = "Test Antique",
                Description = "No image test",
                Price = 25.0m,
                Status = Status.Available,
                ImageFiles = null
            };

            // Act
            var result = await AntiqueHandlers.CreateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                dto,
                mockLogger.Object,
                mockFileService.Object
            );

            // Assert
            var badRequest = Assert.IsType<BadRequest<string>>(result.Result);
            Assert.Equal("ImageFiles must be provided.", badRequest.Value);
        }

        [Fact]
        public async Task CreateAntiqueAsync_ReturnsUnprocessableEntity_WhenFileTooLarge()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var dto = new AntiqueForCreationDto
            {
                Name = "Large File Antique",
                Description = "File too big",
                Price = 100.0m,
                Status = Status.Available,
                ImageFiles = new FormFileCollection
                {
                    FormFileHelpers.CreateFormFile("big.jpg", length: 6 * 1024 * 1024) // 6 MB
                }
            };

            // Act
            var result = await AntiqueHandlers.CreateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                dto,
                mockLogger.Object,
                mockFileService.Object
            );

            // Assert
            Assert.IsType<UnprocessableEntity<string>>(result.Result);
        }

        [Fact]
        public async Task CreateAntiqueAsync_ReturnsStatus500_WhenFileUploadFails()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            mockRepo.Setup(r => r.AddAntiqueAsync(It.IsAny<Antique>()))
                .ThrowsAsync(new Exception("DB failure"));
            var dto = new AntiqueForCreationDto
            {
                Name = "Fail Upload Antique",
                Description = "File upload should fail",
                Price = 75.0m,
                Status = Status.Available,
                ImageFiles = new FormFileCollection
                {
                    FormFileHelpers.CreateFormFile("test.jpg")
                }
            };

            // Act
            var result = await AntiqueHandlers.CreateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                dto,
                mockLogger.Object,
                mockFileService.Object
            );

            // Assert
            Assert.IsType<StatusCodeHttpResult>(result.Result);
            Assert.Equal(500, ((StatusCodeHttpResult)result.Result).StatusCode);
        }
        
        [Fact]
        public async Task UpdateAntiqueAsync_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var antiqueId = 1;
            var updatedDto = new AntiqueForUpdateDto
            {
                Name = "Restored Vase",
                Description = "Now looks great",
                Price = 150.0m,
                Status = Status.Sold,
                ThumbnailFile = "9c6c1c92-8c88-41d9-b13d-672e9a114c5d.png"
            };
            var expectedResponse = new AntiqueForResponseDto
            {
                Name = "Restored Vase",
                Description = "Now looks great",
                Status = Status.Sold,
                Price = 150.0m,
                Thumbnail = "9c6c1c92-8c88-41d9-b13d-672e9a114c5d.png",
                Images = new[]
                {
                    "3f8b1e1c-6c4f-4a4d-9e1f-8c3d82a1f5e4.jpg",
                    "9c6c1c92-8c88-41d9-b13d-672e9a114c5d.png"
                }
            };

            // Act
            var result = await AntiqueHandlers.UpdateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                antiqueId,
                updatedDto,
                mockLogger.Object
            );

            // Assert
            var okResult = Assert.IsType<Ok<AntiqueForResponseDto>>(result.Result);
            Assert.Equivalent(expectedResponse, okResult.Value);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task UpdateAntiqueAsync_ReturnsNotFound_WhenAntiqueDoesNotExist()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var antiqueId = 99;
            var updatedDto = new AntiqueForUpdateDto { Name = "Ghost Item" };

            // Act
            var result = await AntiqueHandlers.UpdateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                antiqueId,
                updatedDto,
                mockLogger.Object
            );

            // Assert
            var notFoundResult = Assert.IsType<NotFound<string>>(result.Result);
            Assert.Equal("Unable to retrieve antique with ID: 99", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateAntiqueAsync_ReturnsUnprocessableEntity_WhenExceptionOccurs()
        {
            // Arrange
            var mockRepo = AntiqueRepositoryMock.GetAntiqueRepository();
            var antiqueId = 2;
            var updatedDto = new AntiqueForUpdateDto { Name = "Still Broken" };
            // Simulate DB failure on save
            mockRepo.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await AntiqueHandlers.UpdateAntiqueAsync(
                mockRepo.Object,
                mockMapper,
                antiqueId,
                updatedDto,
                mockLogger.Object
            );

            // Assert
            Assert.IsType<UnprocessableEntity>(result.Result);
        }
    }
}