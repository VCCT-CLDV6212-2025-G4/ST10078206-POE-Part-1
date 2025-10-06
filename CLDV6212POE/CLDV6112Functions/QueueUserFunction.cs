using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CLDV6112Functions
{
    public class QueueUserFunction
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [Function("QueueUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "QueueUser")] HttpRequestData req)
        {
            var response = req.CreateResponse();

            try
            {
                // Read request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // Connect to Queue
                QueueClient queueClient = new QueueClient(_connectionString, "usertransactions");
                await queueClient.CreateIfNotExistsAsync();

                // Send message to queue
                await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(requestBody)));

                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteStringAsync($"Message added to queue successfully.");
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

