using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;
using WebApplication2.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class BlobService : IStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobService(IConfiguration config)
        {
            var connection = Environment.GetEnvironmentVariable("AZURE_STORAGE_KEY");

            if (string.IsNullOrEmpty(connection))
                throw new Exception("AZURE_STORAGE_KEY environment variable not set");

            var client = new BlobServiceClient(connection);

            _container = client.GetBlobContainerClient("book-images");
            _container.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = _container.GetBlobClient(fileName); //creates a client object representing a single blob inside that container

            using var stream = file.OpenReadStream();

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                }
            };

            await blobClient.UploadAsync(stream, options);

            return blobClient.Uri.ToString();
        }
        public async Task DeleteFileAsync(string blobUrl)
        {
            var uri = new Uri(blobUrl);

            var blobName = Path.GetFileName(uri.LocalPath);

            var blobClient = _container.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
