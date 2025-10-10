using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ObituaryApp.Services
{
    public interface IBlobService
    {
        Task<string?> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string relativePath);
    }
}
