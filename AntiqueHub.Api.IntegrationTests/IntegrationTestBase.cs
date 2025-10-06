using AntiqueHub.Api.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using AntiqueHub.Api.Services;
using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiqueHub.Api.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _httpClient;
    
    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }
}