using FileService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public interface ILeadRepository
    {
        Task AddLeadAsync(LeadDetails lead);
        Task<IEnumerable<LeadDetails>> GetAllLeadsAsync();
        Task<LeadDetails> GetLeadByEmailAsync(string email);
        Task DeleteLeadAsync(string email);

    }
}
