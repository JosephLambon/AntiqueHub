using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using AntiqueHub.Api.Services;
using DocumentMiddleware.Core.Models;

namespace AntiqueHub.Api.Services;
public class FileService(IWebHostEnvironment environment, ILogger<Antique> logger) : IFileService
{
    public async Task<string> UploadFileAsync(
        IFormFile? imageFile,
        string[] allowedFileExtensions
        )
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(imageFile));
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        // Needs updating to automatically configure local/remote setup
        logger.LogInformation("Connecting to storage container...");
        // BlobServiceClient blobServiceClient = BlobStorageService.GetBlobContainerClient(Routes.BlobStorage.STORAGE_ACCOUNT_NAME);
        BlobContainerClient blobContainerClient = BlobStorageService.GetBlobContainerClientLocal();
        
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";

        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName); // Create a new blob for image file
        
        await using (var stream = imageFile.OpenReadStream())
        {
            logger.LogInformation("Uploading blob...");
            await blobClient.UploadAsync(stream, true); // true causes overwrite of existing same fileName
        };
        
        logger.LogInformation("Blob upload succeeded.");
        return fileName;
    }
}