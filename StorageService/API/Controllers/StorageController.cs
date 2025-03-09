using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
using Helpers;
using System.Collections.Concurrent;

namespace API.Controllers
{
    [ApiController]
    [Route("api/storage")]
    public class StorageController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _baseStorageUrl;

        // In-memory dictionary to store metadata (Replace this with DB or file storage)
        private static readonly ConcurrentDictionary<string, FileMetadataDto> _fileMetadataStore = new();

        public StorageController(IConfiguration config)
        {
            _config = config;
            _baseStorageUrl = "http://localhost:5001/api/storage/upload"; // Base URL of storage
        }

        [HttpPost("signed-url")]
        public IActionResult GetSignedUrl([FromBody] FileMetadataDto request)
        {
            var fileId = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o"); // ISO 8601 Format
            var uploadUrl = $"{_baseStorageUrl}?fileId={fileId}&expiry={expiry}";

            var signature = UrlSigner.GenerateSignature(uploadUrl, expiry);
            var signedUrl = $"{uploadUrl}&signature={Uri.EscapeDataString(signature)}";

            // Store metadata
            _fileMetadataStore[fileId] = new FileMetadataDto
            {
                FileName = request.FileName,
                MimeType = request.MimeType,
                Size = request.Size
            };

            return Ok(new { signedUrl, fileId });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string fileId, [FromQuery] string expiry, [FromQuery] string signature)
        {
            if (file == null) return BadRequest("No file provided");

            var uploadUrl = $"{_baseStorageUrl}?fileId={fileId}&expiry={expiry}";

            if (!UrlSigner.ValidateSignature(uploadUrl, expiry, signature))
                return Unauthorized("Invalid signature");

            if (DateTime.TryParse(expiry, out var expiryDate) && expiryDate < DateTime.UtcNow)
                return Unauthorized("URL expired");

            // Validate file metadata
            if (!_fileMetadataStore.TryGetValue(fileId, out var expectedMetadata))
                return BadRequest("Invalid file ID or metadata missing");

            if (file.ContentType != expectedMetadata.MimeType)
                return BadRequest($"Invalid MIME type. Expected: {expectedMetadata.MimeType}, Got: {file.ContentType}");

            if (file.Length != expectedMetadata.Size)
                return BadRequest($"Invalid file size. Expected: {expectedMetadata.Size}, Got: {file.Length}");

            var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

            // Ensure the directory exists
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            // Extract file extension
            var fileExtension = Path.GetExtension(file.FileName);

            var filePath = Path.Combine(storagePath, fileId + fileExtension);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileId, message = "File uploaded successfully" });
        }


        [HttpGet("exists/{fileId}")]
        public IActionResult CheckFileExists(string fileId)
        {
            if (!_fileMetadataStore.TryGetValue(fileId, out var metadata))
                return NotFound(new { fileId, message = "File metadata not found" });

            var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

            // Check all possible file extensions
            string[] possibleExtensions = { ".jpg", ".png" }; // Extend based on expected file types
            foreach (var ext in possibleExtensions)
            {
                var filePath = Path.Combine(storagePath, fileId + ext);
                if (System.IO.File.Exists(filePath))
                {
                    return Ok(new { fileId, exists = true, fileName = metadata.FileName, mimeType = metadata.MimeType });
                }
            }

            return NotFound(new { fileId, exists = false, message = "File not found" });
        }


    }
}
