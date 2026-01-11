using AntiqueHub.Api.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using AntiqueHub.Api.Services;
using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AntiqueHub.Api.IntegrationTests;

public class CustomWebApplicationFactory(PostgreSqlContainerFixture sharedFixture, AzuriteContainerFixture azuriteFixture) : WebApplicationFactory<Program>
{
    public PostgreSqlContainerFixture SharedFixture => sharedFixture; 

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AntiqueDbContext>));
            services.RemoveAll(typeof(AntiqueDbContext));
        
            services.AddDbContext<AntiqueDbContext>(opts =>
                opts.UseNpgsql(sharedFixture.DatabaseConnectionString));

        });
        
        builder.UseSetting("BlobStorage:Host", azuriteFixture.Host);
        builder.UseSetting("BlobStorage:Port", azuriteFixture.Port.ToString());
    }
}