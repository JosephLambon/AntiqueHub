using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using AntiqueHub.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AntiqueHub.Api.IntegrationTests;

public class AntiqueTests : IntegrationTestBase
{
    public AntiqueTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetAntiqueAsync_ReturnsAntiques()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());
        // Act
        var result = await _httpClient.GetAsync("/antiques");
        var content = await result.Content.ReadAsStringAsync();
        
        var antiques = JsonSerializer.Deserialize<List<AntiqueForResponseDto>>(
            content, 
            options
            );
        // Assert
        Assert.NotNull(antiques);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.All(antiques, a => Assert.False(string.IsNullOrWhiteSpace(a.Name)));
    }
}