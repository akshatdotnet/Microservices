using FileService.Infrastructure;
using FileService.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FileService.Application.Interfaces;

namespace FileService.Application
{
    public class FileService : IFileService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ILogger<FileService> _logger;
        
        public FileService(ILeadRepository leadRepository, IFileStorage fileStorage, ILogger<FileService> logger)
        {
            _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Upload file and save lead details
        public async Task<LeadDetails> UploadFileAsync(IFormFile file, LeadDetails leadDetails, HttpRequest request)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Attempted to upload an empty or null file.");
                throw new ArgumentException("File cannot be null or empty.", nameof(file));
            }

            if (leadDetails == null || !leadDetails.IsValid())
            {
                _logger.LogWarning("Invalid lead details provided.");
                throw new ArgumentException("Lead details are invalid.", nameof(leadDetails));
            }

            try
            {
                // Upload the file
                _logger.LogInformation("Uploading file to storage...");
                var fileName = await _fileStorage.SaveFileAsync(file);

                // Construct the file URL
                var fileUrl = $"{request.Scheme}://{request.Host}/file-uploads/{fileName}";
                leadDetails.Document = fileUrl;
                leadDetails.UploadedAt = DateTime.UtcNow; // Use UTC for consistency

                // Save lead details
                _logger.LogInformation("Saving lead details to repository...");
                await Task.Run(() => _leadRepository.AddLeadAsync(leadDetails));

                _logger.LogInformation("File uploaded and lead details saved successfully.");
                return leadDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file or saving lead details.");
                throw new ApplicationException("An error occurred while processing the file upload.", ex);
            }
        }

        // Get all leads
        public async Task<IEnumerable<LeadDetails>> GetAllLeads()
        {
            try
            {
                return await _leadRepository.GetAllLeadsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving leads.");
                throw new ApplicationException("An error occurred while retrieving leads.", ex);
            }
        }

        // Get lead by email
        public async Task<LeadDetails> GetLeadByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Email is missing or invalid.");
                throw new ArgumentException("Email cannot be empty.", nameof(email));
            }

            try
            {
                var lead = await _leadRepository.GetLeadByEmailAsync(email);
                if (lead == null)
                {
                    _logger.LogWarning($"No lead found with email: {email}");
                    return null;
                }
                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the lead for email: {email}");
                throw new ApplicationException($"An error occurred while retrieving the lead for email: {email}", ex);
            }

            
        }

        // Delete lead by email
        public async Task DeleteLead(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Email is missing or invalid.");
                throw new ArgumentException("Email cannot be empty.", nameof(email));
            }
            try
            {
                var lead = await _leadRepository.GetLeadByEmailAsync(email);
                if (lead == null)
                {
                    _logger.LogWarning($"No lead found with email: {email}");
                    throw new ArgumentException("Lead not found.");
                }

                // Delete associated file from storage
                _logger.LogInformation($"Deleting file for lead with email: {email}");
                //await _fileStorage.DeleteFileAsync(lead.Document);
                 _fileStorage.DeleteFile(lead.Document);

                // Delete lead from the repository
                _logger.LogInformation($"Deleting lead with email: {email}");
                await _leadRepository.DeleteLeadAsync(email);

                _logger.LogInformation($"Lead with email: {email} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the lead for email: {email}");
                throw new ApplicationException($"An error occurred while deleting the lead for email: {email}", ex);
            }            
        }
        
    }
}
