using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAPI.SERVICES.Services
{
    public interface IFileService
    {
        public Task<string> UploadImage(string Location, IFormFile file);
        public Task<IActionResult> GetImageAsync(string location, string fileName);
        public void DeleteImage(string path);

    }
}
