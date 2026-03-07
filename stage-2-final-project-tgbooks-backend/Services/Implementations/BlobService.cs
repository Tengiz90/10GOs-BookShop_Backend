using Azure.Storage.Blobs;
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
            var client = new BlobServiceClient(connection);

            _container = client.GetBlobContainerClient("book-images");
            _container.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = _container.GetBlobClient(fileName); //creates a client object representing a single blob inside that container

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream);

            return blobClient.Uri.ToString();
        }
    }
}
