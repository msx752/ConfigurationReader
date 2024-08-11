using Shared.Kernel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Kernel.Repositories.Interfaces
{
    public interface IExceptionLogRepository
    {
        Task AddAsync(ExceptionLog exceptioLog);
        Task<List<ExceptionLog>> ListAsync(int pageNumber = 1, int pageSize = 100);
    }
}