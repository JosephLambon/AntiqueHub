using Testcontainers.Azurite;

namespace AntiqueHub.Api.IntegrationTests;

public class AzuriteContainerFixture : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        .Build();

    public string GetConnectionString() => _azuriteContainer.GetConnectionString();

    public string Host => _azuriteContainer.Hostname;

    public int Port => _azuriteContainer.GetMappedPublicPort(10000);
    
    public async Task InitializeAsync() => await _azuriteContainer.StartAsync();

    public async Task DisposeAsync() => await _azuriteContainer.DisposeAsync();
}