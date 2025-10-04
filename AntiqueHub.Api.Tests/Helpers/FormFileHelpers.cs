using Microsoft.AspNetCore.Http;

namespace AntiqueHub.Api.Tests.Helpers;
public static class FormFileHelpers
{
    public static IFormFile CreateFormFile(string fileName, long length = 1024)
    {
        var stream = new MemoryStream(new byte[length]);
        return new FormFile(stream, 0, length, "file", fileName);
    }
}