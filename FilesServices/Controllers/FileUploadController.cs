using Azure.Storage.Blobs;
using FileService.Domain;
using FileService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilesServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        #region Private variable
        private readonly BlobServiceClient _blobServiceClient;

        private readonly string _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file-uploads");
        private readonly LeadRepository _leadRepository;
        #endregion

        #region Constructor
        public FileUploadController(LeadRepository leadRepository)
        {
            //BlobServiceClient blobServiceClient, 
            //_blobServiceClient = blobServiceClient;

            _leadRepository = leadRepository;
            // Ensure the folder exists
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
        }

        #endregion


        #region Local Storage Code
        // 1. Upload File and Lead Details (POST /api/fileupload/upload)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] LeadDetails leadDetails)
        {
            // Validation: Check if the file is null or empty
            if (file == null || file.Length == 0)
                return BadRequest("File must be provided.");

            // Validation: Ensure lead details are provided and valid
            if (string.IsNullOrEmpty(leadDetails.Username) || string.IsNullOrEmpty(leadDetails.Mobile) || string.IsNullOrEmpty(leadDetails.Email))
            {
                return BadRequest("Lead details (Username, Mobile, Email) must be provided.");
            }

            // Generate a unique file name
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_fileStoragePath, fileName);


            // Save the file locally
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving file: {ex.Message}");
            }

            // Update lead details with the KYC document path and upload time
            leadDetails.Document = $"{Request.Scheme}://{Request.Host}/file-uploads/{fileName}";
            leadDetails.UploadedAt = DateTime.Now;

            // Save the lead details to the repository
            try
            {
                _leadRepository.AddLead(leadDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving lead details: {ex.Message}");
            }

            // Return success response with lead details and file URL
            return Ok(new
            {
                leadDetails.Username,
                FileUrl = leadDetails.Document
            });

        }

        // 2. Get All Leads (GET /api/fileupload)
        [HttpGet]
        public IActionResult GetAllLeads()
        {
            var leads = _leadRepository.GetAllLeads();
            return Ok(leads);
        }

        // 3. Get Lead by Email (GET /api/fileupload/{email})
        [HttpGet("{email}")]
        public IActionResult GetLead(string email)
        {
            // Validation: Check if email is provided
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email must be provided.");
            }
            var lead = _leadRepository.GetLead(email);
            if (lead == null)
                return NotFound("Lead not found.");

            return Ok(lead);
        }

        // 4. Delete Lead and File (DELETE /api/fileupload/{email})
        [HttpDelete("{email}")]
        public IActionResult DeleteLead(string email)
        {
            // Validation: Check if email is provided
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email must be provided.");
            }
            var lead = _leadRepository.GetLead(email);
            if (lead == null)
                return NotFound("Lead not found.");

            // Delete the file from the local storage
            if (System.IO.File.Exists(lead.Document))
            {
                System.IO.File.Delete(lead.Document);
            }

            // Remove lead details from the repository
            _leadRepository.DeleteLead(email);

            return Ok("Lead and associated file deleted successfully.");
        }



        #endregion


        #region Blob Storage Code 

        //// 1. Upload File (POST /api/fileupload)
        //[HttpPost("upload")]
        //[Authorize]
        //public async Task<IActionResult> UploadFile(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var containerClient = _blobServiceClient.GetBlobContainerClient("file-uploads");
        //    await containerClient.CreateIfNotExistsAsync();

        //    var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + "_" + file.FileName);

        //    using (var stream = file.OpenReadStream())
        //    {
        //        await blobClient.UploadAsync(stream, true);
        //    }

        //    return Ok(new { FileName = file.FileName, BlobUrl = blobClient.Uri });
        //}

        //// 2. Get All Files Metadata (GET /api/fileupload)
        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetAllFiles()
        //{
        //    var containerClient = _blobServiceClient.GetBlobContainerClient("file-uploads");
        //    var blobs = containerClient.GetBlobsAsync();
        //    var fileInfos = new List<object>();

        //    await foreach (var blobItem in blobs)
        //    {
        //        fileInfos.Add(new { blobItem.Name, blobItem.Properties.LastModified });
        //    }

        //    return Ok(fileInfos);
        //}

        //// 3. Get File by Name (GET /api/fileupload/{fileName})
        //[HttpGet("{fileName}")]
        //[Authorize]
        //public IActionResult GetFile(string fileName)
        //{
        //    var containerClient = _blobServiceClient.GetBlobContainerClient("file-uploads");
        //    var blobClient = containerClient.GetBlobClient(fileName);

        //    if (!blobClient.Exists())
        //        return NotFound("File not found.");

        //    return Ok(blobClient.Uri);
        //}

        //// 4. Delete File (DELETE /api/fileupload/{fileName})
        //[HttpDelete("{fileName}")]
        //[Authorize]
        //public async Task<IActionResult> DeleteFile(string fileName)
        //{
        //    var containerClient = _blobServiceClient.GetBlobContainerClient("file-uploads");
        //    var blobClient = containerClient.GetBlobClient(fileName);

        //    if (!blobClient.Exists())
        //        return NotFound("File not found.");

        //    await blobClient.DeleteAsync();
        //    return Ok("File deleted successfully.");
        //}

        #endregion


    }
}
