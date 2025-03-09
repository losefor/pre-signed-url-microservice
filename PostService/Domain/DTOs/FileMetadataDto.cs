using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class FileMetadataDto
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
    }
}
