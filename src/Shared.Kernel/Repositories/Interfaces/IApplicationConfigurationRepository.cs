using ConfigurationLib.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Kernel.Repositories.Interfaces
{
    public interface IApplicationConfigurationRepository
    {
        Task AddAsync(ApplicationConfiguration configuration);
        Task<ApplicationConfiguration> GetByIdAsync(ObjectId id);
        Task<List<ApplicationConfiguration>> ListAsync(bool onlyActiveRecods = false, int pageNumber = 1, int pageSize = 100);
        Task UpdateAsync(ApplicationConfiguration configuration);
    }
}