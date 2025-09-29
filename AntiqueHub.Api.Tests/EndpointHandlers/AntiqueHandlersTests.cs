using AutoMapper;
using AntiqueHub.Core.Models;
using AntiqueHub.Api.Profiles;
using AntiqueHub.Tests.Mocks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using AntiqueHub.Api.EndpointsHandlers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AntiqueHub.Tests
{
    public class GetAntiquesAsyncTests
    {
        private readonly ILoggerFactory mockLoggerFactory;
        private readonly IMapper mockMapper;

        public GetAntiquesAsyncTests()
        {
            mockLoggerFactory = NullLoggerFactory.Instance;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AntiqueProfile());
            }, mockLoggerFactory);
            mockMapper = config.CreateMapper();
        }

        [Fact]
        public async Task ReturnsAvailableAntiquesByDefault()
        {
            // Arrange
            var mockContext = AntiqueDbContextMock.CreateMockAntiqueDbContext();
            var mockLogger = new Mock<ILogger<Antique>>();

            // Act
            var result = await AntiqueHandlers.GetAntiquesAsync(
                mockContext.Object,
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
        public async Task ReturnsSoldAntiques_WhenIncludeSoldIsTrue()
        {
            // Arrange
            var mockContext = AntiqueDbContextMock.CreateMockAntiqueDbContext();
            var mockLogger = new Mock<ILogger<Antique>>();

            // Act
            var result = await AntiqueHandlers.GetAntiquesAsync(
                mockContext.Object,
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
        public async Task ReturnsAllStatuses_WhenAllFlagsTrue()
        {
            // Arrange
            var mockContext = AntiqueDbContextMock.CreateMockAntiqueDbContext();
            var mockLogger = new Mock<ILogger<Antique>>();

            // Act
            var result = await AntiqueHandlers.GetAntiquesAsync(
                mockContext.Object,
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
    }
}