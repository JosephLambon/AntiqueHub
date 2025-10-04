using AntiqueHub.Api.Services;
using AntiqueHub.Api.Tests.Helpers;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AntiqueHub.Api.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<ILogger<FileService>> _loggerMock;
        private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
        private readonly Mock<BlobContainerClient> _containerClientMock;
        private readonly Mock<BlobClient> _blobClientMock;

        public FileServiceTests()
        {
            _loggerMock = new Mock<ILogger<FileService>>();
            _blobStorageServiceMock = new Mock<IBlobStorageService>();
            _containerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();

            // Setup default blob behavior
            _blobStorageServiceMock
                .Setup(s => s.GetBlobContainerClient())
                .Returns(_containerClientMock.Object);

            _containerClientMock
                .Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Returns(_blobClientMock.Object);

            _blobClientMock
                .Setup(b => b.UploadAsync(
                    It.IsAny<Stream>(),
                    true,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());
        }

        
        
        [Fact]
        public async Task UploadFileAsync_ShouldThrow_WhenImageFileIsNull()
        {
            // Arrange
            var service = new FileService(_loggerMock.Object, _blobStorageServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.UploadFileAsync(null, new[] { ".jpg", ".png" }));
        }

        [Fact]
        public async Task UploadFileAsync_ShouldThrow_WhenExtensionNotAllowed()
        {
            // Arrange
            var service = new FileService(_loggerMock.Object, _blobStorageServiceMock.Object);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.UploadFileAsync(fileMock.Object, new[] { ".jpg", ".png" }));
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnFileName_WhenValidFileUploaded()
        {
            // Arrange
            var service = new FileService(_loggerMock.Object, _blobStorageServiceMock.Object);

            var fileMock = FormFileHelpers.CreateFormFile("big.jpg", length: 4 * 1024 * 1024);

            // Act
            var result = await service.UploadFileAsync(fileMock, new[] { ".jpg", ".png" });

            // Assert
            Assert.EndsWith(".jpg", result);

            _blobStorageServiceMock.Verify(s => s.GetBlobContainerClient(), Times.Once);
            _containerClientMock.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Once);
            _blobClientMock.Verify(b =>
                b.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
