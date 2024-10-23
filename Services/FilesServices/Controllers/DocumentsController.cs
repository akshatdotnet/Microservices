using Microsoft.AspNetCore.Mvc;
using FileService.Domain;
using FileService.Application.Interfaces;
using FileService.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;



namespace FileService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IFileService _fileService;

        public DocumentsController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // 1. Upload File and Lead Details (POST /api/documents/upload)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] LeadDetails leadDetails)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is missing or empty.");
            }

            if (leadDetails == null || string.IsNullOrWhiteSpace(leadDetails.Email))
            {
                return BadRequest("Lead details are missing or invalid.");
            }

            try
            {
                var uploadedLead = await _fileService.UploadFileAsync(file, leadDetails,Request);
                return Ok(new { uploadedLead.Username, FileUrl = uploadedLead.Document });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        // 2. Get All Leads (GET /api/documents)
        [HttpGet]
        public async Task<IActionResult> GetAllLeads()
        {
            try
            {
                //var leads = await _fileService.GetAllLeadsAsync();
                var leads =  await Task.Run(() => _fileService.GetAllLeads());
                return Ok(leads);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the leads.");
            }
        }

        // 3. Get Lead by Email (GET /api/documents/{email})
        [HttpGet("{email}")]
        public async Task<IActionResult> GetLeadByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                //var lead = await _fileService.GetLeadByEmailAsync(email);
                var lead =  await Task.Run(() => _fileService.GetLeadByEmail(email));
                if (lead == null)
                {
                    return NotFound($"No lead found with the email: {email}");
                }

                return Ok(lead);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the lead.");
            }
        }

        // 4. Delete Lead and File (DELETE /api/documents/{email})
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteLead(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                //await _fileService.DeleteLeadAsync(email);
                 await Task.Run(() => _fileService.DeleteLead(email));
                return Ok("Lead and associated file deleted successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the lead.");
            }
        }
    }
}
