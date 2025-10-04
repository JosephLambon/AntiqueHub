using Azure.Storage.Blobs;
using AntiqueHub.Core.Models;

namespace AntiqueHub.Api.Services;
public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly BlobContainerClient _containerClient;

    public FileService(ILogger<FileService> logger, IBlobStorageService blobStorageService)
    {
        _logger = logger;
        _containerClient = blobStorageService.GetBlobContainerClient();
    }
    public async Task<string> UploadFileAsync(
        IFormFile? imageFile,
        string[] allowedFileExtensions
        )
    {
        ArgumentNullException.ThrowIfNull(imageFile);
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }
        
        var fileName = $"{Guid.NewGuid().ToString()}{ext}"; 
        BlobClient blobClient = _containerClient.GetBlobClient(fileName); // Create a new blob for image file

        await using (var stream = imageFile.OpenReadStream())
        {
            _logger.LogInformation("Uploading blob...");
            await blobClient.UploadAsync(stream, true); // true causes overwrite of existing same fileName
        };
        
        _logger.LogInformation("Blob upload succeeded.");
        return fileName;
    }
}