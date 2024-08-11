using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using Shared.Kernel.Data;
using Shared.Kernel.Models;
using Shared.Kernel.Repositories.Interfaces;

namespace Shared.Kernel.Repositories
{
    public class ExceptionLogRepository : IExceptionLogRepository
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The mongo db context.
        /// </summary>
        private readonly IMongoDbContext _mongoDbContext;

        public ExceptionLogRepository(IMongoDbContext mongoDbContext, ILogger logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }

        public async Task AddAsync(ExceptionLog exceptioLog)
        {
            try
            {
                if (exceptioLog == null)
                    throw new ArgumentNullException(nameof(exceptioLog));

                if (exceptioLog.Id != ObjectId.Empty)
                    throw new ArgumentNullException(nameof(exceptioLog.Id));

                if (string.IsNullOrWhiteSpace(exceptioLog.ApplicationName))
                    throw new ArgumentNullException(nameof(exceptioLog.ApplicationName));


                await _mongoDbContext.ExceptionLogs.InsertOneAsync(exceptioLog);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<List<ExceptionLog>> ListAsync(int pageNumber = 1, int pageSize = 100)
        {
            try
            {
                if (pageNumber < 1)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 100;

                if (pageSize > 200)
                    pageSize = 200;

                int skip = (pageNumber - 1) * pageSize;

                var result = await _mongoDbContext.ExceptionLogs
                    .Find(Builders<ExceptionLog>.Filter.Empty)
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
    }
}