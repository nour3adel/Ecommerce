
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class FileService : IFileService
    {
        #region Fields

        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Constructors
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region UploadImage Service
        public async Task<string> UploadImage(string location, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "NoImage";
            }
            if (file.Length > 1 * 1024 * 1024)
            {
                return "ImageSizeExceeded";
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return "InvalidFileType";
            }

            location = Path.Combine("Uploads", location);
            var path = Path.Combine(_webHostEnvironment.WebRootPath, location);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(path, fileName);

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(filePath))
                {
                    DeleteImage(filePath);
                }

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                return $"/{location}/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return "FailedToUploadImage";
            }
        }


        #endregion

        #region GetImage Service
        public async Task<IActionResult> GetImageAsync(string location, string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", location, fileName);
            if (File.Exists(filePath))
            {
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return new FileContentResult(fileBytes, "image/jpeg");
            }
            else
            {
                return new NotFoundObjectResult("Image not found");
            }
        }

        #endregion

        #region Delete Image Service
        public void DeleteImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null or empty.");
            }

            int index = path.IndexOf("Uploads", StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                throw new ArgumentException("The path does not contain 'Uploads'.", nameof(path));
            }

            string relativePath = path.Substring(index).Replace("\\", "/");
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine("File deleted successfully.");
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                throw new InvalidOperationException("An error occurred while deleting the image.", ex);
            }
        }
        #endregion

    }
}
