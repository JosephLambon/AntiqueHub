using AntiqueHub.Api.Services;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;

namespace AntiqueHub.Api.Extensions;

public static class BlobStorageExtensions
{
    public static IServiceCollection AddBlobStorageService(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        BlobContainerClient containerClient;

        if (env.IsDevelopment())
        {
            // Account key is the Azurite default accountKey - works for anyone
            var credential = new StorageSharedKeyCredential(
                "devstoreaccount1",
                "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="
            );

            var blobHost = configuration["BlobStorage:Host"];
            var containerName = configuration["BlobStorage:ContainerName"];
            var localUri = new Uri($"http://{blobHost}:10000/devstoreaccount1/{containerName}");
            containerClient = new BlobContainerClient(localUri, credential);
        }
        else
        {
            var accountName = configuration["BlobStorage:AccountName"];
            var containerName = configuration["BlobStorage:ContainerName"];
            containerClient = new BlobContainerClient(
                new Uri($"https://{accountName}.blob.core.windows.net/{containerName}"),
                new DefaultAzureCredential()
            );
        }

        try
        {
            var createContainerTask = containerClient.CreateIfNotExistsAsync();
            createContainerTask.Wait();
        } catch (Exception ex)
        {
            Console.WriteLine($"Error creating blob container: {ex.Message}");
            throw;
        }

        services.AddSingleton(new BlobStorageService(containerClient));
        services.AddSingleton<IBlobStorageService>(sp => sp.GetRequiredService<BlobStorageService>());

        return services;
    }
}