using Microsoft.AspNetCore.Http.Json;
using AntiqueHub.Core.Models;
using System.Text.Json.Serialization;
using AntiqueHub.Api.Services;

namespace AntiqueHub.Api.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<AntiqueDbContext>();
        services.AddTransient<IFileService, FileService>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                    ;
                });
        });

        // Defaults enum results to be returned as strings
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        // Same as above, needed for Swagger response to update. Result of conflict in MinimalAPI/Swashbuckle
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddProblemDetails();
        return services;
    }
}