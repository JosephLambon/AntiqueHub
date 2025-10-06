using System.Net.Http.Json;

namespace AntiqueHub.Api.IntegrationTests;

public class AntiqueHubApiEndpointsHelper(HttpClient httpClient)
{
    private HttpClient _httpClient => httpClient;

    // public async Task<HttpResponseMessage> GetAntiForgeryToken()
    // {
    //     return await _httpClient.GetAsync();
    // }


}