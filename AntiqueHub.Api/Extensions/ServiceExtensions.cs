using AntiqueHub.Core.Repositories;
using AntiqueHub.Core.Models;
using System.Text.Json.Serialization;
using AntiqueHub.Api.Services;
using AntiqueHub.Core.Interfaces;


namespace AntiqueHub.Api.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<AntiqueDbContext>();
        services.AddScoped<IAntiqueRepository, AntiqueRepository>();
        services.AddTransient<IFileService, FileService>();

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
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                    ;
                });
        });
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAntiforgery(options =>
        {
            options.FormFieldName = "AFFormField";
            options.HeaderName = "X-CSRF-TOKEN"; 
        });
        return services;
    }
}