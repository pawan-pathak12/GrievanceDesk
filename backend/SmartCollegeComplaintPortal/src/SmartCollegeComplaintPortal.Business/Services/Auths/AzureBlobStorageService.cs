using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SmartCollegeComplaintPortal.Business.Configurations;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;

namespace SmartCollegeComplaintPortal.Business.Services.Auths
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(
            IOptions<AzureBlobStorageOptions> options)
        {
            var settings = options.Value;

            var blobServiceClient = new BlobServiceClient(
                settings.ConnectionString);

            _containerClient = blobServiceClient.GetBlobContainerClient(
                settings.ContainerName);
        }

        public async Task<string> UploadAsync(Stream fileStream,
      string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
        {
            string extension = Path.GetExtension(fileName);

            string storedFileName =
     $"{Guid.NewGuid()}{extension}";

            string blobPath =
                $"{folder}/{storedFileName}";

            BlobClient blobClient =
         _containerClient.GetBlobClient(blobPath);

            await blobClient.UploadAsync(
                fileStream,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                },
                cancellationToken: cancellationToken);

            return blobPath;
        }

        public async Task DeleteAsync(
        string blobPath,
        CancellationToken cancellationToken = default)
        {
            BlobClient blobClient =
                _containerClient.GetBlobClient(blobPath);

            await blobClient.DeleteIfExistsAsync(
                cancellationToken: cancellationToken);
        }
    }
}
