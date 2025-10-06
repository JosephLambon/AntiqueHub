using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using AntiqueHub.Api.Services;

namespace AntiqueHub.Api.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    //
    // public IntegrationTestBase(WebApplicationFactory<Program> factory)
    // {
    //     _httpClient = factory
    //         .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
    //         {
    //             services.AddScoped<IFileService, FileService>();
    //         }))
    // }
}