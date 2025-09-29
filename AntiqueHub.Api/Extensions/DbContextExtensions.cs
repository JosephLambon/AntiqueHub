using DocumentMiddleware.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace AntiqueHub.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DocumentDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("default"));
        });
        return services;
    }
}