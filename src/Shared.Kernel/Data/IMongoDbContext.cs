using ConfigurationLib.Models;
using MongoDB.Driver;
using Shared.Kernel.Models;

namespace Shared.Kernel.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<ApplicationConfiguration> Configurations { get; }
        IMongoCollection<ExceptionLog> ExceptionLogs { get; }
    }
}