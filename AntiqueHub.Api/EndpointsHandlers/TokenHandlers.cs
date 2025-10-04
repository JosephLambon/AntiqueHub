using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using AntiqueHub.Core.Entities;

namespace AntiqueHub.Api.EndpointsHandlers;

public static class TokenHandlers
{
    public static async Task<IResult> GetAntiForgeryToken(
        [FromServices] IAntiforgery antiForgery, HttpContext http,  [FromServices] ILogger<Antique> logger) 
    {
        logger.LogInformation("GET /antiforgery-token request received...");
        var tokens = antiForgery.GetAndStoreTokens((http));
        return TypedResults.Ok(
            new { token = tokens.RequestToken });
    }
}