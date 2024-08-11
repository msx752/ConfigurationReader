using ConfigurationLib.Abstracts;
using ConfigurationLib.Models;
using MongoDB.Driver;
using Shared.Kernel.Models;
using System;

namespace Shared.Kernel.Data
{
    /// <summary>
    /// The mongo db context.
    /// </summary>
    public class MongoDbContext : IMongoDbContext
    {
        /// <summary>
        /// The database.
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// The lazyapplication configurations.
        /// </summary>
        private readonly Lazy<IMongoCollection<ApplicationConfiguration>> _lazy_applicationConfigurations;

        /// <summary>
        /// The lazyexception logs.
        /// </summary>
        private readonly Lazy<IMongoCollection<ExceptionLog>> _lazy_exceptionLogs;

        /// <summary>
        /// The mongo client.
        /// </summary>
        private readonly MongoClient _mongoClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MongoDbContext(string connectionString)
        {
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(MongoDbConfigurationReader.databaseName);
            _lazy_applicationConfigurations = new Lazy<IMongoCollection<ApplicationConfiguration>>(() => _database.GetCollection<ApplicationConfiguration>(MongoDbConfigurationReader.collectionName));
            _lazy_exceptionLogs = new Lazy<IMongoCollection<ExceptionLog>>(() => _database.GetCollection<ExceptionLog>("ExceptionLogs"));
        }

        /// <summary>
        /// Gets the configurations.
        /// </summary>
        /// <value>A ımongocollection of applicationconfigurations.</value>
        public IMongoCollection<ApplicationConfiguration> Configurations => _lazy_applicationConfigurations.Value;

        /// <summary>
        /// Gets the exception logs.
        /// </summary>
        /// <value>A ımongocollection of exceptiologs.</value>
        public IMongoCollection<ExceptionLog> ExceptionLogs => _lazy_exceptionLogs.Value;
    }
}