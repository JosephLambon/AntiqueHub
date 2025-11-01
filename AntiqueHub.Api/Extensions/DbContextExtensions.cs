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
            var conn = configuration.GetConnectionString("psqlProduction")
               ?? Environment.GetEnvironmentVariable("CUSTOMCONNSTR_psqlProduction")
               ?? Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_psqlProduction");
            // Defined in app service environment variables
            Console.WriteLine($"DEBUG: psqlProduction = {conn}");


            services.AddDbContext<AntiqueDbContext>(options =>
            {
                options.UseNpgsql(conn);
            });
        }
        return services;
    }
}