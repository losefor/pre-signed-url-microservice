using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class StorageService : IStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _storageServiceUrl;
        private readonly ILogger<StorageService> _logger;

        public StorageService(HttpClient httpClient, IConfiguration config, ILogger<StorageService> logger)
        {
            _httpClient = httpClient;
            _storageServiceUrl = config["StorageService:Url"] ?? throw new ArgumentNullException("StorageService:Url is missing in configuration");
            _logger = logger;
        }

        public async Task<FileMetadataResponse?> GetFileMetadataAsync(string fileId)
        {
            var requestUrl = $"{_storageServiceUrl}/exists/{fileId}";
            _logger.LogInformation("Checking file existence: {Url}", requestUrl);

            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("File {FileId} not found", fileId);
                return null;
            }

            return JsonSerializer.Deserialize<FileMetadataResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
