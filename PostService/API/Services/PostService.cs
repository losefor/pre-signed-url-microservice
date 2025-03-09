using System.Collections.Concurrent;
using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class PostService
    {
        private static readonly ConcurrentDictionary<string, PostDto> _postStore = new();
        private readonly IStorageService _storageService;
        private readonly ILogger<PostService> _logger;

        public PostService(IStorageService storageService, ILogger<PostService> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<PostDto?> CreatePostAsync(CreatePostDto request)
        {
            if (string.IsNullOrEmpty(request.FileId))
                throw new ArgumentException("FileId is required");

            var fileMetadata = await _storageService.GetFileMetadataAsync(request.FileId);
            if (fileMetadata == null)
                return null; // Handle file-not-found scenario in controller

            var postId = Guid.NewGuid().ToString();
            var newPost = new PostDto
            {
                Id = postId,
                Title = request.Title,
                Content = request.Content,
                FileId = request.FileId
            };

            _postStore[postId] = newPost;
            _logger.LogInformation("Post {PostId} created", postId);

            return newPost;
        }

        public PostDto? GetPostById(string postId) => _postStore.TryGetValue(postId, out var post) ? post : null;
    }
}
