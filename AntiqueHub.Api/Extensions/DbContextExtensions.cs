using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace AntiqueHub.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.AddDbContext<AntiqueDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("psqlLocal"));
            });
        }
        else
        {
            // Defined in app service environment variables
            services.AddDbContext<AntiqueDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("psqlProduction"));
            });
        }
        return services;
    }
}