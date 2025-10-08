using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using AntiqueHub.Core.Entities;
using AntiqueHub.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;

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
        var X_CSRF_TOKEN =
            "CfDJ8F0ngc7gY-dKnTnYb_jZlWKFhDyfOJIJjJhflAamc_AfT9hk-qtm-Q6j409f3_pg4VPxG9xuzSOxLRVQMcVa1c4cbYR7SQr8QSzzqxU-QrQu1dn4hH6KgfIN640k4Ej1HcPsgMBviEhkDxC0v9Zrxc4";
        _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", X_CSRF_TOKEN);
        var imageContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
    
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Vintage Clock"), "Name" },
            { new StringContent("A lovely antique clock"), "Description" },
            { new StringContent("100.00"), "Price" },
            { new StringContent("Available"), "Status" },
            { imageContent, "ImageFiles", "clock.jpg" }
        };

        // Act
        var result = await _httpClient.PostAsync("/antiques", formData);
        var content = await result.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Contains("/antiques/", result.Headers.Location?.ToString());
        Assert.False(string.IsNullOrWhiteSpace(content));
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