using MassTransit;
using System;
using System.Threading.Tasks;

namespace Shared.Kernel.Bus
{
    public interface IMessageBus
    {
        Task PublishEvent<T>(T @event) where T : CorrelatedBy<Guid>;
    }
}