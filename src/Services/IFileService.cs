using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Melodify.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        void DeleteFile(string relativePath);
    }
}
