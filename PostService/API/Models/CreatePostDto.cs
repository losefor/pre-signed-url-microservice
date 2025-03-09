namespace API.Models
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string FileId { get; set; }
    }
}
