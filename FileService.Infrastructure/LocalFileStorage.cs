using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _storagePath;

        private readonly string _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file-uploads");
        public LocalFileStorage(string storagePath)
        {
            //_storagePath = storagePath;
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));

        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File must be provided.");

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            //var filePath = Path.Combine(_storagePath, fileName);
            //var filePath = $"{Request.Scheme}://{Request.Host}/file-uploads/{fileName}";
            var filePath = Path.Combine(_fileStoragePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
       
    }
}
