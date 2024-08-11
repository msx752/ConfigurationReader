using Shared.Kernel.Messages;
using System.Threading.Tasks;

namespace ConfigurationLib.LogService.Logics
{
    public interface IExceptionLogLogic
    {
        Task LogAsync(ExceptionLogEvent @event);
    }
}