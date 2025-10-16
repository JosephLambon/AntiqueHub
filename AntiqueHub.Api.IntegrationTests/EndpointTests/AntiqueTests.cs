// using System.Net;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using AntiqueHub.Core.Entities;
// using AntiqueHub.Core.Models;
// using Microsoft.AspNetCore.Mvc.Testing;
// using System.Net.Http.Headers;
// using System.Text;
//
// namespace AntiqueHub.Api.IntegrationTests;
//
// public class AntiqueTests : WebApplicationFactory<Program>
// {
//     private readonly JsonSerializerOptions _jsonOptions;
//     
//     public AntiqueTests(WebApplicationFactory<Program> factory) : base(factory)
//     {
//         _jsonOptions = new JsonSerializerOptions
//         {
//             PropertyNameCaseInsensitive = true
//         };
//         _jsonOptions.Converters.Add(new JsonStringEnumConverter());
//     }
//
//     [Theory]
//     [InlineData(true, true, true)]
//     [InlineData(true, false, true)]
//     [InlineData(true, false, false)]
//     [InlineData(false, true, true)]
//     [InlineData(false, true, false)]
//     [InlineData(false, false, true)]
//     [InlineData(false, false, false)]
//     public async Task GetAntiqueAsync_ReturnsAntiques_WhenStatusSpecified(
//         bool IncludeAvailable,
//         bool IncludeSold,
//         bool IncludeArchived
//         )
//     {
//         // Arrange
//         var expectedStatuses = new List<Status?>();
//         if (IncludeAvailable)
//             expectedStatuses.Add(Status.Available);
//         if (IncludeSold)
//             expectedStatuses.Add(Status.Sold);
//         if (IncludeArchived)
//             expectedStatuses.Add(Status.Archived);
//         
//         // Act
//         var result = await _httpClient.GetAsync($"antiques/?includeSold={IncludeSold}&includeAvailable={IncludeAvailable}&includeArchived={IncludeArchived}");
//         var content = await result.Content.ReadAsStringAsync();
//         var antiques = JsonSerializer.Deserialize<List<AntiqueForResponseDto>>(
//             content, 
//             _jsonOptions
//             );
//         
//         // Assert
//         Assert.NotNull(antiques);
//         Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//         Assert.IsType<List<AntiqueForResponseDto>>(antiques);
//         Assert.All(antiques, a=> Assert.Contains(a.Status, expectedStatuses));
//         if (IncludeAvailable == false && IncludeSold == false && IncludeArchived == false)
//             Assert.Empty(antiques);
//     }
//     
//     [Fact]
//     public async Task GetAntiqueByIdAsync_ReturnsAntique_WhenExists()
//     {
//         // Arrange & Act
//         var result = await _httpClient.GetAsync("/antiques/1");
//         var content = await result.Content.ReadAsStringAsync();
//         var antique = JsonSerializer.Deserialize<AntiqueForResponseDto>(content, _jsonOptions);
//
//         // Assert
//         Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//         Assert.NotNull(antique);
//         Assert.Equal(1, antique.Id);
//         Assert.False(string.IsNullOrWhiteSpace(antique.Name));
//     }
//
//     [Fact]
//     public async Task GetAntiqueByIdAsync_ReturnsNotFound_WhenDoesNotExist()
//     {
//         // Arrange
//         var result = await _httpClient.GetAsync("/antiques/9999");
//     
//         // Assert
//         Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
//     }
//     
//     [Fact]
//     public async Task CreateAntiqueAsync_CreatesAntique_WhenValid()
//     {
//         // Arrange
//         await AntiforgeryTokenHelper.SetAntiforgeryTokenAsync(_httpClient);
//         
//         var imageContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
//         imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
//         using var formData = new MultipartFormDataContent("----boundary");
//         formData.Add(new StringContent("Vintage Clock"), "Name");
//         formData.Add(new StringContent("A lovely antique clock"), "Description");
//         formData.Add(new StringContent("100.00"), "Price");
//         formData.Add(new StringContent("Available"), "Status");
//         formData.Add(imageContent, "ImageFiles", "clock.jpg");
//         
//         // Act
//         var result = await _httpClient.PostAsync("/antiques", formData);
//         var content = await result.Content.ReadAsStringAsync();
//         var antiqueResponse  = JsonSerializer.Deserialize<AntiqueForResponseDto>(content);
//
//         // Assert
//         Assert.Equal(HttpStatusCode.Created, result.StatusCode);
//         Assert.Contains("/antiques/", result.Headers.Location?.ToString()); // Location header contains /antiques/{id}
//         Assert.False(string.IsNullOrWhiteSpace(content)); // Returns non-empty body
//     }
//
//     [Fact]
//     public async Task CreateAntiqueAsync_ReturnsBadRequest_WhenNoImages()
//     {
//         // Arrange
//         await AntiforgeryTokenHelper.SetAntiforgeryTokenAsync(_httpClient);
//         var formData = new MultipartFormDataContent
//         {
//             { new StringContent("Old Vase"), "Name" },
//             { new StringContent("Antique vase without image"), "Description" },
//             { new StringContent("Available"), "Status" },
//             { new StringContent("500"), "Price" }
//         };
//
//         // Act
//         var result = await _httpClient.PostAsync("/antiques", formData);
//         var content = await result.Content.ReadAsStringAsync();
//         var responseMessage = JsonSerializer.Deserialize<string>(content, _jsonOptions);
//         
//         // Assert
//         Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
//         Assert.Equal("ImageFiles must be provided.", responseMessage);
//     }
//     
//     [Fact]
//     public async Task UpdateAntiqueAsync_UpdatesAntique_WhenValid()
//     {
//         // Arrange
//         var antiqueId = 2;
//         var jsonBody = """
//                        {
//                         "description": "A lovely old thing with an updated description."
//                        }
//                        """;
//         var content = new StringContent(
//             jsonBody,
//             Encoding.UTF8,
//             "application/json"
//         );
//         
//         
//         // Act
//         var result = await _httpClient.PatchAsync($"/antiques/{antiqueId}", content);
//         var responseContent = await result.Content.ReadAsStringAsync();
//         var antiqueResponse  = JsonSerializer.Deserialize<AntiqueForResponseDto>(responseContent, _jsonOptions);
//         
//         // Assert
//         Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//         Assert.Equivalent(antiqueId, antiqueResponse.Id);
//         Assert.Equivalent("A lovely old thing with an updated description.",
//             antiqueResponse.Description);
//     }
//     [Fact]
//     public async Task UpdateAntiqueAsync_ReturnsNotFound_WhenSentInvalidId()
//     {
//         // Arrange
//         var antiqueId = 999999;
//         var jsonBody = """
//                        {
//                         "name": "Update antique name"
//                        }
//                        """;
//         var content = new StringContent(
//             jsonBody,
//             Encoding.UTF8,
//             "application/json"
//         );
//         
//         // Act
//         var result = await _httpClient.PatchAsync($"/antiques/{antiqueId}", content);
//         var responseContent = await result.Content.ReadAsStringAsync();
//         var responseMessage = JsonSerializer.Deserialize<string>(responseContent);
//
//         // Assert
//         Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
//         Assert.Equal($"Unable to retrieve antique with ID: {antiqueId}", responseMessage);
//     }
// }