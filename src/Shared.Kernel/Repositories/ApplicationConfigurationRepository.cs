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
using Shared.Kernel.Repositories.Interfaces;

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

        public async Task AddAsync(ApplicationConfiguration configuration)
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

                var response = (await _mongoDbContext.Configurations
                 .FindAsync(f => f.ApplicationName.ToLowerInvariant() == configuration.ApplicationName.ToLowerInvariant()
                                && f.Name.ToLowerInvariant() == configuration.Name.ToLowerInvariant()
                                && f.IsActive)
                 ).FirstOrDefault();

                if (response != null && configuration.IsActive)
                    throw new InvalidOperationException($"given '{configuration.Name}' name exists for the ApplicationName: '{configuration.ApplicationName}' and has Active state, you cannot add anoher active state for the same key.");

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
        public async Task<ApplicationConfiguration> GetByIdAsync(ObjectId id)
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

        public async Task UpdateAsync(ApplicationConfiguration configuration)
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

                var response = (await _mongoDbContext.Configurations
                 .FindAsync(f => f.ApplicationName.ToLowerInvariant() == configuration.ApplicationName.ToLowerInvariant() 
                                && f.Name.ToLowerInvariant() == configuration.Name.ToLowerInvariant() 
                                && f.IsActive && f.Id != configuration.Id)
                 ).FirstOrDefault();

                if (response != null && configuration.IsActive)
                    throw new InvalidOperationException($"given '{configuration.Name}' name exists for the ApplicationName: '{configuration.ApplicationName}' and has Active state, you cannot set anoher active state for the same key.");

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