using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Domain;
using Microsoft.AspNetCore.Http;


namespace FileService.Application.Interfaces
{
    public interface IFileService
    {
        Task<LeadDetails> UploadFileAsync(IFormFile file, LeadDetails leadDetails, HttpRequest request);
        Task<IEnumerable<LeadDetails>> GetAllLeads();
        Task<LeadDetails> GetLeadByEmail(string email);
        Task DeleteLead(string email);
    }
}
