using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Domain;

namespace FileService.Infrastructure
{
    public class LeadRepository : ILeadRepository
    {

        private readonly List<LeadDetails> _leads = new List<LeadDetails>();

        public async Task AddLeadAsync(LeadDetails lead)
        {
            // Simulate async behavior for in-memory repository
            await Task.Run(() => _leads.Add(lead));
        }

        public async Task<IEnumerable<LeadDetails>> GetAllLeadsAsync()
        {
            return await Task.FromResult(_leads);
        }

        public async Task<LeadDetails> GetLeadByEmailAsync(string email)
        {
            var lead = _leads.FirstOrDefault(l => l.Email == email);
            return await Task.FromResult(lead);
        }

        public async Task DeleteLeadAsync(string email)
        {
            var lead = _leads.FirstOrDefault(l => l.Email == email);
            if (lead != null)
            {
                await Task.Run(() => _leads.Remove(lead));
            }
        }


    }
}
