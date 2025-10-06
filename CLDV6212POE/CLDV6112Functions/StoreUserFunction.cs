using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Data.Tables;
using CLDV6112Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace CLDV6112Functions
{
    public class StoreUserFunction
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [Function("StoreUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "StoreUser")] HttpRequestData req)
        {
            var response = req.CreateResponse();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Users newUser = JsonConvert.DeserializeObject<Users>(requestBody);

                if (newUser == null || string.IsNullOrEmpty(newUser.User_Id))
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    await response.WriteStringAsync("Invalid user object in request body.");
                    return response;
                }

                if (string.IsNullOrEmpty(newUser.PartitionKey))
                    newUser.PartitionKey = "UserPartition";

                if (string.IsNullOrEmpty(newUser.RowKey))
                    newUser.RowKey = Guid.NewGuid().ToString();

                TableClient tableClient = new TableClient(_connectionString, "User");
                await tableClient.CreateIfNotExistsAsync();
                await tableClient.AddEntityAsync(newUser);

                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteStringAsync($"User {newUser.User_Id} stored successfully.");
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


