using ConfigurationLib.Interfaces;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Shared.Kernel.Bus
{
    /// <summary>
    /// The message bus.
    /// </summary>
    public class MessageBus: IMessageBus
    {
        /// <summary>
        /// Publish endpoint.
        /// </summary>
        private readonly IPublishEndpoint _publishEndpoint;

        /// <summary>
        /// The configuration reader.
        /// </summary>
        private readonly IConfigurationReader _configurationReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus"/> class.
        /// </summary>
        /// <param name="publishEndpoint">The publish endpoint.</param>
        public MessageBus(IPublishEndpoint publishEndpoint, IConfigurationReader configurationReader)
        {
            _publishEndpoint = publishEndpoint;
            _configurationReader = configurationReader;
        }

        /// <summary>
        /// Publish the event.
        /// </summary>
        /// <typeparam name="T"/>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task PublishEvent<T>(T @event) where T : CorrelatedBy<Guid>
        {
            if (!_configurationReader.GetValue<bool>("IsloggingEnabled"))
                return;

            await _publishEndpoint.Publish(@event);
        }
    }
}
