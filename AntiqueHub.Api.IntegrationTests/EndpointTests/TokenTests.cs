using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AntiqueHub.Api.IntegrationTests;

public class TokenTests : IntegrationTestBase
{
    public TokenTests(WebApplicationFactory<Program> factory) : base(factory) { }
    
    [Fact]
    public async Task GetToken_ReturnsToken()
    {
        // Arrange & Act
        var result = _httpClient.GetAsync("/antiforgery-token");
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.Result.StatusCode);
    }
}