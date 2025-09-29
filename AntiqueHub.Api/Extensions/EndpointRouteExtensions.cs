using AntiqueHub.Api.EndpointsHandlers;
using DocumentMiddleware.Core.Constants;

namespace AntiqueHub.Api.Extensions
{
    public static class EndpointRouteExtensions
    {
        public static void RegisterAntiqueEndpoints(this IEndpointRouteBuilder app)
        {
            var imageEndpoints = app.MapGroup("/antiques")
                .WithOpenApi()
                .WithTags("Antique endpoints");

            imageEndpoints.MapGet("", AntiqueHandlers.GetAntiquesAsync)
                .WithSummary("Retrieve antiques based on their status");
            
            imageEndpoints.MapPost("", AntiqueHandlers.CreateAntiqueAsync)
                .WithSummary("Add new antique")
                .DisableAntiforgery();
            
            imageEndpoints.MapGet("/{antiqueId}", AntiqueHandlers.GetAntiqueByIdAsync)
                .WithName(Routes.GET_ANTIQUE_BY_ID)
                .WithSummary("Retrieve an antique by its ID");
            
            imageEndpoints.MapPatch("/{antiqueId}", AntiqueHandlers.UpdateAntiqueAsync) 
                .WithSummary("Update an antique entity by ID");
        }
    }
}
