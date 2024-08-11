using ConfigurationLib.Models;
using MongoDB.Driver;

namespace ConfigurationLib.Dashboard.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<ApplicationConfiguration> Configurations { get; }
    }
}