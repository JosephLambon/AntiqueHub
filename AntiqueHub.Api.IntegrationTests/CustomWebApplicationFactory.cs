using AntiqueHub.Api.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using AntiqueHub.Api.Services;
using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiqueHub.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly HttpClient _httpClient;
    private readonly string _postgresConnectionString;
    
    public CustomWebApplicationFactory(string postgreSqlConnectionString)
    {
        _httpClient = CreateClient();
        _postgresConnectionString = postgreSqlConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Find and remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AntiqueDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);
            
            services.AddDbContext<AntiqueDbContext>(options =>
            {
                options.UseNpgsql(_postgresConnectionString);
            });
        });
    }
}