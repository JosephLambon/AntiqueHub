using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AntiqueHub.Api.IntegrationTests;
using AntiqueHub.Core.Entities;
using AntiqueHub.Core.Models;

[Collection(nameof(IntegrationTestsCollection))]
public class AntiqueIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AntiqueIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }
    
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
        bool IncludeArchived)
    {
        var expectedStatuses = new List<Status?>();
        if (IncludeAvailable) expectedStatuses.Add(Status.Available);
        if (IncludeSold) expectedStatuses.Add(Status.Sold);
        if (IncludeArchived) expectedStatuses.Add(Status.Archived);

        var result = await _client.GetAsync(
            $"antiques/?includeSold={IncludeSold}&includeAvailable={IncludeAvailable}&includeArchived={IncludeArchived}");

        var content = await result.Content.ReadAsStringAsync();
        var antiques = JsonSerializer.Deserialize<List<AntiqueForResponseDto>>(content, _jsonOptions);

        Assert.NotNull(antiques);
        if (!IncludeAvailable && !IncludeSold && !IncludeArchived)
            Assert.Empty(antiques);
        else
            Assert.NotEmpty(antiques);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<List<AntiqueForResponseDto>>(antiques);
        Assert.All(antiques, a => Assert.Contains(a.Status, expectedStatuses));
    }
    
    [Fact]
    public async Task GetAntiqueByIdAsync_ReturnsAntique_WhenExists()
    {
        // Arrange
        var antiqueId = 1;
        var expectedAntique = new AntiqueForResponseDto()
        {
            Id = 1,
            Name = "First antique",
            Status = Status.Available,
            Description = "A lovely old thing with a description.",
            Price = 500m,
            Thumbnail = "fa453816-bb2b-45d4-83ce-3c5e99ccba25.jpg",
            Images = new[]
            {
                "fa453816-bb2b-45d4-83ce-3c5e99ccba25.jpg"
            }
        };
        
        // Act
        var result = await _client.GetAsync($"/antiques/{antiqueId}");
        var content = await result.Content.ReadAsStringAsync();
        var antique = JsonSerializer.Deserialize<AntiqueForResponseDto>(content, _jsonOptions);
        
        
        
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(antique);
        Assert.Equivalent(expectedAntique, antique);
        Assert.Equal(1, antique.Id);
        Assert.False(string.IsNullOrWhiteSpace(antique.Name));
    }

    [Fact]
    public async Task GetAntiqueByIdAsync_ReturnsNotFound_WhenDoesNotExist()
    {
        var result = await _client.GetAsync("/antiques/9999");
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task CreateAntiqueAsync_CreatesAntique_WhenValid()
    {
        await AntiforgeryTokenHelper.SetAntiforgeryTokenAsync(_client);

        var imageContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Bespoke vintage Clock"), "Name");
        formData.Add(new StringContent("A lovely antique clock"), "Description");
        formData.Add(new StringContent("100.00"), "Price");
        formData.Add(new StringContent("Available"), "Status");
        formData.Add(imageContent, "ImageFiles", "clock.jpg");

        var result = await _client.PostAsync("/antiques", formData);
        var content = await result.Content.ReadAsStringAsync();
        var antiqueResponse = JsonSerializer.Deserialize<AntiqueForResponseDto>(content, _jsonOptions);

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Contains("/antiques/", result.Headers.Location?.ToString());
        Assert.False(string.IsNullOrWhiteSpace(content));
    }

    [Fact]
    public async Task CreateAntiqueAsync_ReturnsBadRequest_WhenNoImages()
    {
        await AntiforgeryTokenHelper.SetAntiforgeryTokenAsync(_client);

        var formData = new MultipartFormDataContent
        {
            { new StringContent("Old Vase"), "Name" },
            { new StringContent("Antique vase without image"), "Description" },
            { new StringContent("Available"), "Status" },
            { new StringContent("500"), "Price" }
        };

        var result = await _client.PostAsync("/antiques", formData);
        var content = await result.Content.ReadAsStringAsync();
        var responseMessage = JsonSerializer.Deserialize<string>(content, _jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("ImageFiles must be provided.", responseMessage);
    }

    [Fact]
    public async Task UpdateAntiqueAsync_UpdatesAntique_WhenValid()
    {
        var antiqueId = 2;
        var jsonBody = """
                       {
                        "description": "A lovely old thing with an updated description."
                       }
                       """;

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var result = await _client.PatchAsync($"/antiques/{antiqueId}", content);
        var responseContent = await result.Content.ReadAsStringAsync();
        var antiqueResponse = JsonSerializer.Deserialize<AntiqueForResponseDto>(responseContent, _jsonOptions);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equivalent(antiqueId, antiqueResponse.Id);
        Assert.Equivalent("A lovely old thing with an updated description.", antiqueResponse.Description);
    }

    [Fact]
    public async Task UpdateAntiqueAsync_ReturnsNotFound_WhenSentInvalidId()
    {
        var antiqueId = 999999;
        var jsonBody = """
                       {
                        "name": "Update antique name"
                       }
                       """;

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var result = await _client.PatchAsync($"/antiques/{antiqueId}", content);
        var responseContent = await result.Content.ReadAsStringAsync();
        var responseMessage = JsonSerializer.Deserialize<string>(responseContent, _jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal($"Unable to retrieve antique with ID: {antiqueId}", responseMessage ); 
    }
}