using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CLDV6112Functions
{
    public class UploadToFileShareFunction
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [Function("UploadToFileShare")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadToFileShare")] HttpRequestData req)
        {
            var response = req.CreateResponse();

            try
            {
                // Read request body
                string content = await new StreamReader(req.Body).ReadToEndAsync();

                // Connect to Azure File Share
                string shareName = "userfiles"; // Name of your file share
                ShareClient share = new ShareClient(_connectionString, shareName);
                await share.CreateIfNotExistsAsync();

                // Create a directory (optional)
                string directoryName = "users";
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
                await directory.CreateIfNotExistsAsync();

                // Generate file name
                string fileName = $"User_{Guid.NewGuid()}.txt";
                ShareFileClient file = directory.GetFileClient(fileName);

                byte[] byteArray = Encoding.UTF8.GetBytes(content);

                using MemoryStream stream = new MemoryStream(byteArray);
                await file.CreateAsync(stream.Length);
                await file.UploadRangeAsync(new HttpRange(0, stream.Length), stream);

                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteStringAsync($"File '{fileName}' uploaded successfully to Azure Files.");
            }
            catch (RequestFailedException ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await response.WriteStringAsync($"Azure Storage Error: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await response.WriteStringAsync($"Error: {ex.Message}");
            }

            return response;
        }
    }
}

