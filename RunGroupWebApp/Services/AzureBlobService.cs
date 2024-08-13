using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using RunGroupWebApp.Helpers;
using RunGroupWebApp.Interfaces;

namespace RunGroupWebApp.Services
{
    public class AzureBlobService : IAzureBlobService //handle Azure Blob Storage operations
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        public AzureBlobService(IOptions<AzureStorageConfig> config)
        {
            _blobServiceClient = new BlobServiceClient(config.Value.ConnectionString);
            _containerName = config.Value.ContainerName;
        }


        public async Task<string> UploadPhotoAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);

            return blobClient.Uri.ToString();
        }
    }
}
