
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Hackathon.AI.Models.Settings;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.IO.Pipes;
using Azure;
using System.Collections.Generic;

namespace Hackathon.AI.Services
{

    public class BlobStorageService
    {
        private readonly AzureBlobStorageSettings _settings;

        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IOptions<AzureBlobStorageSettings> settings)
        {
            _settings = settings.Value;
            _containerClient = new BlobContainerClient(new Uri($"{_settings.SasUri}?{_settings.SasToken}"));
        }

        public async Task<BlobContentInfo> UploadFileAsync(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            Console.WriteLine($"Uploading to Blob storage as blob:\n\t {blobClient.Uri}");

            var result = await blobClient.UploadAsync(filePath, true);
            return result.Value;
        }

        //public async Task<BlobContentInfo> UploadFileAsync(Stream fileStream)
        //{
        //    string fileName = Guid.NewGuid().ToString("N");
        //    BlobClient blobClient = _containerClient.GetBlobClient(fileName);

        //    Console.WriteLine($"Uploading to Blob storage as blob:\n\t {blobClient.Uri}");

        //    var result = await blobClient.UploadAsync(fileStream, true);
        //    return result.Value;
        //}

        public async Task<string> UploadFileAsync(Stream fileStream)
        {
            string fileName = Guid.NewGuid().ToString("N");
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            Console.WriteLine($"Uploading to Blob storage as blob:\n\t {blobClient.Uri}");

            var result = await blobClient.UploadAsync(fileStream, true);

            return GetBlobSasUri(blobClient);
        }

        public IEnumerable<string> GetBlobs()
        {
            IEnumerable<BlobItem> blobs = (_containerClient.GetBlobsAsync()).ToBlockingEnumerable();
            return blobs.ToList().Select(b => $"{_settings.SasUri}/{b.Name}?{_settings.SasToken}");
        }

        private string GetBlobSasUri(BlobClient blobClient)
        {
            return $"{_settings.SasUri}/{blobClient.Name}?{_settings.SasToken}";
        }
    }

}
