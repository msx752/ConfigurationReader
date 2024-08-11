using ConfigurationLib.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Shared.Kernel.Data;
using ConfigurationLib;

namespace Shared.Kernel.Repositories
{
    /// <summary>
    /// The application configuration repository.
    /// </summary>
    public class ApplicationConfigurationRepository : IApplicationConfigurationRepository
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The mongo db context.
        /// </summary>
        private readonly IMongoDbContext _mongoDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigurationRepository"/> class.
        /// </summary>
        /// <param name="mongoDbContext">The mongo db context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public ApplicationConfigurationRepository(IMongoDbContext mongoDbContext, ILogger logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }

        public async Task Add(ApplicationConfiguration configuration)
        {
            try
            {
                if (configuration == null)
                    throw new ArgumentNullException(nameof(configuration));

                if (configuration.Id != ObjectId.Empty)
                    throw new ArgumentNullException(nameof(configuration.Id));

                if (string.IsNullOrWhiteSpace(configuration.ApplicationName))
                    throw new ArgumentNullException(nameof(configuration.ApplicationName));

                if (string.IsNullOrWhiteSpace(configuration.Name))
                    throw new ArgumentNullException(nameof(configuration.Name));

                if (string.IsNullOrEmpty(configuration.Value))
                    throw new ArgumentNullException(nameof(configuration.Value));

                var type = Extensions.GetSupportedTypeByStringType(configuration.Type);
                if (type == null)
                    throw new NotSupportedException($"{configuration.Type} type not supported.");

                object configValue = Convert.ChangeType(configuration.Value, type);

                await _mongoDbContext.Configurations.InsertOneAsync(configuration);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                throw;
            }
        }

        /// <summary>
        /// Get by ıd.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> of type <see cref="ApplicationConfiguration"/></returns>
        public async Task<ApplicationConfiguration> GetById(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return null;

            var response = await _mongoDbContext.Configurations.FindAsync(f => f.Id == id);

            var applicationConfiguration = (await response.ToListAsync()).FirstOrDefault();

            return applicationConfiguration;
        }

        /// <summary>
        /// List and return a <see cref="Task"/> of a list of applicationconfigurationdtos.
        /// </summary>
        /// <param name="onlyActiveRecods">If true, only active recods.</param>
        /// <returns>A <see cref="Task"/> of a list of applicationconfigurationdtos</returns>
        public async Task<List<ApplicationConfiguration>> ListAsync(bool onlyActiveRecods = false, int pageNumber = 1, int pageSize = 100)
        {
            try
            {
                if (pageNumber < 1)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 100;

                if (pageSize > 200)
                    pageSize = 200;

                FilterDefinition<ApplicationConfiguration> filter = onlyActiveRecods
                    ? Builders<ApplicationConfiguration>.Filter.Eq(f => f.IsActive, true)
                    : Builders<ApplicationConfiguration>.Filter.Empty;

                int skip = (pageNumber - 1) * pageSize;

                var result = await _mongoDbContext.Configurations
                    .Find(filter)
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task Update(ApplicationConfiguration configuration)
        {
            try
            {
                if (configuration == null)
                    throw new ArgumentNullException(nameof(configuration));

                if (configuration.Id == ObjectId.Empty)
                    throw new ArgumentNullException(nameof(configuration.Id));

                if (string.IsNullOrWhiteSpace(configuration.ApplicationName))
                    throw new ArgumentNullException(nameof(configuration.ApplicationName));

                if (string.IsNullOrWhiteSpace(configuration.Name))
                    throw new ArgumentNullException(nameof(configuration.Name));

                if (string.IsNullOrEmpty(configuration.Value))
                    throw new ArgumentNullException(nameof(configuration.Value));

                var type = Extensions.GetSupportedTypeByStringType(configuration.Type);
                if (type == null)
                    throw new NotSupportedException($"{configuration.Type} type not supported.");

                object configValue = Convert.ChangeType(configuration.Value, type);

                var updateDefinition = Builders<ApplicationConfiguration>.Update
                    .Set(f => f.Name, configuration.Name)
                    .Set(f => f.ApplicationName, configuration.ApplicationName)
                    .Set(f => f.IsActive, configuration.IsActive)
                    .Set(f => f.Type, configuration.Type)
                    .Set(f => f.Value, configuration.Value);

                await _mongoDbContext.Configurations.FindOneAndUpdateAsync(f => f.Id == configuration.Id, updateDefinition);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                throw;
            }
        }
    }
}