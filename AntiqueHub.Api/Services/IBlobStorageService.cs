using Azure.Storage.Blobs;

namespace AntiqueHub.Api.Services;

public interface IBlobStorageService
{
    BlobContainerClient GetBlobContainerClient();
}