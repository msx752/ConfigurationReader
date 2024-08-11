using ConfigurationLib.Interfaces;
using ConfigurationLib.LogService.Logics;
using MassTransit;
using Shared.Kernel.Messages;
using System.Threading.Tasks;

namespace ConfigurationLib.LogService.Consumers
{
    public class ExceptionLogConsumer : IConsumer<ExceptionLogEvent>
    {
        private readonly Serilog.ILogger _logger;
        private readonly IExceptionLogLogic _exceptionLogLogic;
        private readonly IConfigurationReader _configurationReader;

        public ExceptionLogConsumer(IExceptionLogLogic logDetailLogic, IConfigurationReader configurationReader, Serilog.ILogger logger)
        {
            _exceptionLogLogic = logDetailLogic;
            _configurationReader = configurationReader;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ExceptionLogEvent> context)
        {
            if (_configurationReader.GetValue<bool>("IsBrokerMessageShown"))
                _logger.Information($"wew error event arrived, short message:'{context.Message.ExceptionMessage}'");

            await _exceptionLogLogic.LogAsync(context.Message);
        }
    }
}
