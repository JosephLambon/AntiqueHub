using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;

namespace AntiqueHub.Api.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }
        public BlobContainerClient GetBlobContainerClient() => _containerClient;
    }
}
