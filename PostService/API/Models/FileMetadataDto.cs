namespace API.Models
{
    public class FileMetadataResponse
    {
        public string FileId { get; set; }
        public bool Exists { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
    }
}
