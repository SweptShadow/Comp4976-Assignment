using System;
using System.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ObituaryApp.Services
{
    public class BlobService : IBlobService
    {
        private readonly string? _connectionString;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("AzureBlob:ConnectionString");
            _containerName = configuration.GetValue<string>("AzureBlob:ContainerName") ?? "uploads";
        }

        public async Task<string?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                // No Azure configured, caller should fallback to local storage
                return null;
            }

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                var headers = new BlobHttpHeaders { ContentType = file.ContentType };
                await blobClient.UploadAsync(stream, headers);
            }

            // Return the relative path/URL (use blob's Uri)
            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            if (string.IsNullOrWhiteSpace(_connectionString)) return;

            try
            {
                var blobServiceClient = new BlobServiceClient(_connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                // If relativePath is a full Uri, extract blob name
                var uri = new Uri(relativePath);
                var blobName = Path.GetFileName(uri.LocalPath);

                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch
            {
                // ignore errors on delete
            }
        }
    }
}
