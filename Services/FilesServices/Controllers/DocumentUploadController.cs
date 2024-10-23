using Microsoft.AspNetCore.Mvc;
using FileService.Domain;
using FileService.Infrastructure;
using System.Collections.Generic;
using FileService.Application;
using Microsoft.AspNetCore.Authorization;

namespace FileService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentUploadController : Controller
    {
        private readonly IFileService _fileService;

        public DocumentUploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // 1. Upload File and Lead Details (POST /api/fileupload/upload)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] LeadDetails leadDetails)
        {
            try
            {
                var lead = await _fileService.UploadFileAsync(file, leadDetails, Request);
                return Ok(new { lead.Username, FileUrl = lead.Document });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. Get All Leads (GET /api/fileupload)
        [HttpGet]
        public IActionResult GetAllLeads()
        {
            var leads = _fileService.GetAllLeads();
            return Ok(leads);
        }

        // 3. Get Lead by Email (GET /api/fileupload/{email})
        [HttpGet("{email}")]
        public IActionResult GetLead(string email)
        {
            var lead = _fileService.GetLeadByEmail(email);
            if (lead == null)
            {
                return NotFound("Lead not found.");
            }

            return Ok(lead);
        }

        // 4. Delete Lead and File (DELETE /api/fileupload/{email})
        [HttpDelete("{email}")]
        public IActionResult DeleteLead(string email)
        {
            try
            {
                _fileService.DeleteLead(email);
                return Ok("Lead and associated file deleted successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
