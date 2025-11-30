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
            var conn = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_psqlProduction");
            services.AddDbContext<AntiqueDbContext>(options =>
            {
                options.UseNpgsql(conn);
            });
        }
        return services;
    }
}