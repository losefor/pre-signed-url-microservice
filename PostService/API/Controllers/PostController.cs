using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(PostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto request)
        {
            try
            {
                var post = await _postService.CreatePostAsync(request);
                if (post == null)
                    return NotFound(new { message = "The attached file does not exist or is not uploaded." });

                return Ok(new { post.Id, message = "Post created successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid post request");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{postId}")]
        public IActionResult GetPost(string postId)
        {
            var post = _postService.GetPostById(postId);
            return post == null
                ? NotFound(new { postId, message = "Post not found" })
                : Ok(post);
        }
    }
}
