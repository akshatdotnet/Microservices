using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;



namespace FilesServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageFilesController : ControllerBase
    {
        private readonly string _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file-uploads");

        public ManageFilesController()
        {
            // Ensure the folder exists
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
        }


        // 1. Upload File (POST /api/fileupload)
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(_fileStoragePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/file-uploads/{fileName}";
            return Ok(new { FileName = file.FileName, FileUrl = fileUrl });
        }

        // 2. Get All Files Metadata (GET /api/fileupload)
        [HttpGet]
        [Authorize]
        public IActionResult GetAllFiles()
        {
            var fileInfos = Directory.GetFiles(_fileStoragePath)
                .Select(file => new
                {
                    FileName = Path.GetFileName(file),
                    LastModified = System.IO.File.GetLastWriteTime(file)
                }).ToList();

            return Ok(fileInfos);
        }

        // 3. Get File by Name (GET /api/fileupload/{fileName})
        [HttpGet("{fileName}")]
        [Authorize]
        public IActionResult GetFile(string fileName)
        {
            var filePath = Path.Combine(_fileStoragePath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileUrl = $"{Request.Scheme}://{Request.Host}/file-uploads/{fileName}";
            return Ok(fileUrl);
        }

        // 4. Delete File (DELETE /api/fileupload/{fileName})
        [HttpDelete("{fileName}")]
        [Authorize]
        public IActionResult DeleteFile(string fileName)
        {
            var filePath = Path.Combine(_fileStoragePath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            System.IO.File.Delete(filePath);
            return Ok("File deleted successfully.");
        }
    }



}

