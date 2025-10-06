using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json.Linq;
using System;

namespace CLDV6112Functions
{
    public class UploadFileFunction
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [Function("UploadFile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadFile")] HttpRequestData req)
        {
            var response = req.CreateResponse();

            try
            {
                // Read request body as string
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject json = JObject.Parse(requestBody);

                // Extract FileName or generate default
                string originalFileName = json["FileName"]?.ToString() ?? "sample.json";

                // Ensure unique filename by adding a GUID
                string fileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

                // Connect to Blob Storage
                BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("files");
                await containerClient.CreateIfNotExistsAsync();

                // Upload the JSON content
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(BinaryData.FromString(requestBody));

                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteStringAsync($"File '{fileName}' uploaded successfully to blob storage.");
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await response.WriteStringAsync($"Error: {ex.Message}");
            }

            return response;
        }
    }
}
