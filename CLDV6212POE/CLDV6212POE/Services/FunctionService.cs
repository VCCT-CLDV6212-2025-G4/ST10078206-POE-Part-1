using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CLDV6212POE.Services
{
    public class FunctionService
    {
        private readonly HttpClient _httpClient;

        public FunctionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Call StoreUser Function
        public async Task<string> StoreUserAsync(object user)
        {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/StoreUser", content);
            return await response.Content.ReadAsStringAsync();
        }

        // Call UploadFile Function
        public async Task<string> UploadFileAsync(object fileData)
        {
            var json = JsonSerializer.Serialize(fileData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/UploadFile", content);
            return await response.Content.ReadAsStringAsync();
        }

        // Call Queue Function
        public async Task<string> QueueUserAsync(object queueData)
        {
            var json = JsonSerializer.Serialize(queueData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/QueueUser", content);
            return await response.Content.ReadAsStringAsync();
        }

        // Call FileShare Function
        public async Task<string> SendFileAsync(object fileData)
        {
            var json = JsonSerializer.Serialize(fileData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/SendFile", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
