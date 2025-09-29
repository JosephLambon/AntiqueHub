using AntiqueHub.Core.Models;

namespace AntiqueHub.Api.Services;
public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile imageFile, string[] allowedFileExtensions);
}