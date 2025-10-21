using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AntiqueHub.Api.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public class TokenIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public TokenIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }
    
    [Fact]
    public async Task GetToken_ReturnsToken()
    {
        // Arrange & Act
        var result = await _client.GetAsync("/antiforgery-token");
        var content = await result.Content.ReadAsStringAsync();
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<string>(content);
        Assert.NotEmpty(content);
    }
}