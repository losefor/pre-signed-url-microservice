using API.Models;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using System.IO;
using System.Threading.Tasks;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/post")]
    public class PostController : ControllerBase
    {
        private readonly StorageService _storageService;

        public PostController(StorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var metadata = new FileMetadataDto
            {
                FileName = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length
            };

            var (signedUrl, fileId) = await _storageService.GetSignedUrlAsync(metadata);

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var responseMessage = await _storageService.UploadFileAsync(signedUrl, fileBytes, file.FileName, file.ContentType);

            return Ok(new { fileId, message = "File uploaded successfully", response = responseMessage });
        }
    }
}
