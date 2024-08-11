using ConfigurationLib.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace ConfigurationLib.Abstracts
{
    /// <summary>
    /// The mongo db configuration reader.
    /// </summary>
    public abstract class MongoDbConfigurationReader : ConfigurationReaderBase
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public const string collectionName = "Configurations";

        /// <summary>
        /// The database name.
        /// </summary>
        public const string databaseName = "ApplicationConfigurationDB";

        /// <summary>
        /// The collection.
        /// </summary>
        private readonly IMongoCollection<ApplicationConfiguration> _collection;

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// The database.
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// The mongo client.
        /// </summary>
        private readonly MongoClient _mongoClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbConfigurationReader"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="refreshTimerIntervalInMs">The refresh timer ınterval ın ms.</param>
        public MongoDbConfigurationReader(string connectionString, int refreshTimerIntervalInMs)
            : base(refreshTimerIntervalInMs)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "parameter value cannot be null or empty.");

            _connectionString = connectionString;

            _mongoClient = InitializeMongoClient(connectionString);

            _database = _mongoClient.GetDatabase(databaseName);
           
            _collection = _database.GetCollection<ApplicationConfiguration>(collectionName);
        }

        /// <summary>
        /// Initializes mongo client.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A <see cref="MongoClient"/></returns>
        protected internal MongoClient InitializeMongoClient(string connectionString)
            => new(connectionString);

        /// <summary>
        /// List configuration by application name.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <returns>A <see cref="Task"/> of type IAsyncCursor</returns>
        protected internal virtual async Task<IAsyncCursor<ApplicationConfiguration>> ListConfigurationByApplicationNameAsync(string applicationName)
        {
            _database.CreateCollection(collectionName);

            return await _collection.FindAsync(f => f.ApplicationName.ToLowerInvariant() == applicationName.ToLowerInvariant() && f.IsActive == true);
        }
    }
}