using MongoDB.Driver;
using System;

namespace ConfigLib.Abstracts
{
    public abstract class MongoDbConfigurationReader : ConfigurationReaderBase
    {
        #region fields

        public const string recordName = "ApplicationConfiguration";
        private readonly string _connectionString;
        private readonly Lazy<IMongoDatabase> _lazy_database;
        private readonly MongoClient _mongoClient;

        #endregion fields

        public MongoDbConfigurationReader(string connectionString, int refreshTimerIntervalInMs)
            : base(refreshTimerIntervalInMs)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "parameter value cannot be null or empty.");

            _connectionString = connectionString;

            _mongoClient = InitializeMongoClient(connectionString);

            _lazy_database = new(() => _mongoClient.GetDatabase(recordName));
        }

        #region protected methods

        protected IMongoDatabase Database => _lazy_database.Value;

        protected MongoClient InitializeMongoClient(string connectionString)
            => new(connectionString);

        #endregion protected methods
    }
}