using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public interface IStorageService
    {
        Task<FileMetadataResponse?> GetFileMetadataAsync(string fileId);
    }
}
