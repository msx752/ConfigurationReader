using ConfigurationLib.Models;
using MongoDB.Driver;

namespace Shared.Kernel.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<ApplicationConfiguration> Configurations { get; }
    }
}