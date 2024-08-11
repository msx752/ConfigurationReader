using AutoMapper;
using Shared.Kernel.Messages;
using Shared.Kernel.Models;

namespace ConfigurationLib.LogService.Profiles
{
    public class ExceptionLogProfiles : Profile
    {
        public ExceptionLogProfiles()
        {
            CreateMap<ExceptionLogEvent, ExceptionLog>();
        }
    }
}
