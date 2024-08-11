using AutoMapper;
using Shared.Kernel.Messages;
using Shared.Kernel.Models;
using Shared.Kernel.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ConfigurationLib.LogService.Logics
{
    public class ExceptionLogLogic : IExceptionLogLogic
    {
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IExceptionLogRepository _repository;

        public ExceptionLogLogic(IExceptionLogRepository repository, IMapper mapper, Serilog.ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task LogAsync(ExceptionLogEvent @event)
        {
            try
            {
                var log = _mapper.Map<ExceptionLog>(@event);
                await _repository.AddAsync(log);
            }
            catch (System.Exception e)
            {
                _logger.Error(e, e.Message);
            }
        }
    }
}