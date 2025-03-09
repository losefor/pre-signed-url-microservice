using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using API.Models;

namespace API.Services
{
    public class StorageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _storageServiceUrl;

        public StorageService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _storageServiceUrl = config["STORAGE_SERVICE_URL"] ?? "http://storage-service:5001";
        }

        public async Task<(string signedUrl, string fileId)> GetSignedUrlAsync(FileMetadataDto metadata)
        {
            var json = JsonSerializer.Serialize(metadata);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_storageServiceUrl}/api/storage/signed-url", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            var signedUrl = root.GetProperty("signedUrl").GetString();
            var fileId = root.GetProperty("fileId").GetString();

            return (signedUrl, fileId);
        }

        public async Task<string> UploadFileAsync(string signedUrl, byte[] fileBytes, string fileName, string mimeType)
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);

            content.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync(signedUrl, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
