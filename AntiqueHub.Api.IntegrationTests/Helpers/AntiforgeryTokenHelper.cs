using System.Text.Json;

namespace AntiqueHub.Api.IntegrationTests;

public static class AntiforgeryTokenHelper
{
    public static async Task SetAntiforgeryTokenAsync(HttpClient _httpClient)
    {
        // Get token
        var tokenResponse = await _httpClient.GetAsync("/antiforgery-token");
        tokenResponse.EnsureSuccessStatusCode();

        var responseJson = await tokenResponse.Content.ReadAsStringAsync();
        var tokenObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
        var antiForgeryToken = tokenObj.GetProperty("token").GetString();
        var cookieHeader = tokenResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();
        _httpClient.DefaultRequestHeaders.Add("Cookie", cookieHeader);
        _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", antiForgeryToken);
    }
}