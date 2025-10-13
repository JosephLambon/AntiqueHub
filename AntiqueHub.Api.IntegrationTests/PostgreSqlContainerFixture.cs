using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace AntiqueHub.Api.IntegrationTests;

public class PostgreSqlContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Postgres {get; private set;}

    public PostgreSqlContainerFixture()
    {
        Postgres = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Postgres.StartAsync();
        // Ensures migrations applied. Should match prod schema
        var options = new DbContextOptionsBuilder<AntiqueDbContext>()
            .UseNpgsql(Postgres.GetConnectionString())
            .Options;

        using var context = new AntiqueDbContext(options);
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Postgres.DisposeAsync().AsTask();
}