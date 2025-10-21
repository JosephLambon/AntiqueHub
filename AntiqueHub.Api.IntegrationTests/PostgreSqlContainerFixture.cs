using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace AntiqueHub.Api.IntegrationTests;

public class PostgreSqlContainerFixture : IAsyncLifetime
{
    private AntiqueDbContext? _dbContext;
    
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("mydatabase")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    
    public string DatabaseConnectionString => _dbContainer.GetConnectionString();
    public AntiqueDbContext AntiqueDbContext => _dbContext;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var options = new DbContextOptionsBuilder<AntiqueDbContext>()
            .UseNpgsql(DatabaseConnectionString)
            .Options;
        _dbContext = new AntiqueDbContext(options);
        
        await _dbContext.Database.MigrateAsync();
        
        await ResetAndSeedDatabaseAsync();
        // Ensure Postgres respects seeded Id's
        await _dbContext.Database.ExecuteSqlRawAsync(
            "SELECT setval(pg_get_serial_sequence('\"Antiques\"', 'Id'), COALESCE(MAX(\"Id\"), 1), true) FROM \"Antiques\";"
            );
    }
    
    private async Task ResetAndSeedDatabaseAsync()
    {
        _dbContext.Antiques.RemoveRange(_dbContext.Antiques);
        await _dbContext.SaveChangesAsync();

        await TestDataHelper.SeedAsync(_dbContext);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}