using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;

namespace AntiqueHub.Api.Services
{
    public static class BlobStorageService
    {
        public static BlobContainerClient GetBlobContainerClient(string accountName)
        {
            var client = new BlobContainerClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                new DefaultAzureCredential()
                );

            return client;
        }
        
        public static BlobContainerClient GetBlobContainerClientLocal()
        {
            var credential = new StorageSharedKeyCredential(
                "devstoreaccount1",
                "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="
                );
            var client = new BlobContainerClient(
                new Uri(String.Concat("http://127.0.0.1:10000/devstoreaccount1/", Constants.BlobStorage.ANTIQUE_IMAGE_CONTAINER)),
                credential
            );
            
            return client;
        }
    }
}
