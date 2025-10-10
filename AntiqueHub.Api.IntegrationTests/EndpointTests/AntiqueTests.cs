using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using AntiqueHub.Core.Entities;
using AntiqueHub.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace AntiqueHub.Api.IntegrationTests;

public class AntiqueTests : IntegrationTestBase
{
    public AntiqueTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    [InlineData(false, false, false)]
    public async Task GetAntiqueAsync_ReturnsAntiques_WhenStatusSpecified(
        bool IncludeAvailable,
        bool IncludeSold,
        bool IncludeArchived
        )
    {
        // Arrange
        var expectedStatuses = new List<Status?>();
        if (IncludeAvailable)
            expectedStatuses.Add(Status.Available);
        if (IncludeSold)
            expectedStatuses.Add(Status.Sold);
        if (IncludeArchived)
            expectedStatuses.Add(Status.Archived);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());
        
        // Act
        var result = await _httpClient.GetAsync($"antiques/?includeSold={IncludeSold}&includeAvailable={IncludeAvailable}&includeArchived={IncludeArchived}");
        var content = await result.Content.ReadAsStringAsync();
        var antiques = JsonSerializer.Deserialize<List<AntiqueForResponseDto>>(
            content, 
            options
            );
        
        // Assert
        Assert.NotNull(antiques);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<List<AntiqueForResponseDto>>(antiques);
        Assert.All(antiques, a=> Assert.Contains(a.Status, expectedStatuses));
        if (IncludeAvailable == false && IncludeSold == false && IncludeArchived == false)
            Assert.Empty(antiques);
    }
    
    [Fact]
    public async Task GetAntiqueByIdAsync_ReturnsAntique_WhenExists()
    {
        // Arrange
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
    
        // Act
        var result = await _httpClient.GetAsync("/antiques/1");
        var content = await result.Content.ReadAsStringAsync();
        var antique = JsonSerializer.Deserialize<AntiqueForResponseDto>(content, options);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(antique);
        Assert.Equal(1, antique.Id);
        Assert.False(string.IsNullOrWhiteSpace(antique.Name));
    }

    [Fact]
    public async Task GetAntiqueByIdAsync_ReturnsNotFound_WhenDoesNotExist()
    {
        // Arrange
        var result = await _httpClient.GetAsync("/antiques/9999");
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task CreateAntiqueAsync_CreatesAntique_WhenValid()
    {
        // Arrange
        // Get token
        var tokenResponse = await _httpClient.GetAsync("/antiforgery-token");
        tokenResponse.EnsureSuccessStatusCode();

        var responseJson = await tokenResponse.Content.ReadAsStringAsync();
        var tokenObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
        var antiForgeryToken = tokenObj.GetProperty("token").GetString();
        var cookieHeader = tokenResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();
        _httpClient.DefaultRequestHeaders.Add("Cookie", cookieHeader);
        _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", antiForgeryToken);
        
        var imageContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        
        using var formData = new MultipartFormDataContent("----boundary");
        formData.Add(new StringContent("Vintage Clock"), "Name");
        formData.Add(new StringContent("A lovely antique clock"), "Description");
        formData.Add(new StringContent("100.00"), "Price");
        formData.Add(new StringContent("Available"), "Status");
        formData.Add(imageContent, "ImageFiles", "clock.jpg");
        
        // Act
        var result = await _httpClient.PostAsync("/antiques", formData);
        var content = await result.Content.ReadAsStringAsync();
        var antiqueResponsezdt0-9  = JsonSerializer.Deserialize<AntiqueForResponseDto>(content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Contains("/antiques/", result.Headers.Location?.ToString()); // Location header contains /antiques/{id}
        Assert.False(string.IsNullOrWhiteSpace(content)); // Returns non-empty body
    }

    [Fact]
    public async Task CreateAntiqueAsync_ReturnsBadRequest_WhenNoImages()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Old Vase"), "Name" },
            { new StringContent("Antique vase without image"), "Description" }
        };

        // Act
        var result = await _httpClient.PostAsync("/antiques", formData);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

}