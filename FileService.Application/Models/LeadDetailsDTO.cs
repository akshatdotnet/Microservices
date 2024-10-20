using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Models
{
    public class LeadDetailsDTO
    {
        public string Username { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string? Document { get; set; }  // Path to the KYC document
        public DateTime UploadedAt { get; set; }

        // Parameterless constructor
        public LeadDetailsDTO() { }
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Email);
        }
    }

    public class FileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
