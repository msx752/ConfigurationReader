using ConfigurationLib.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Kernel.Repositories
{
    public interface IApplicationConfigurationRepository
    {
        Task Add(ApplicationConfiguration configuration);
        Task<ApplicationConfiguration> GetById(ObjectId id);
        Task<List<ApplicationConfiguration>> ListAsync(bool onlyActiveRecods = false, int pageNumber = 1, int pageSize = 100);
        Task Update(ApplicationConfiguration configuration);
    }
}