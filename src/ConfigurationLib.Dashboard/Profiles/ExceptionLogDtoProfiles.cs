using AutoMapper;
using ConfigurationLib.Dashboard.Models.Dtos;
using Shared.Kernel.Models;

namespace ConfigurationLib.Dashboard.Profiles
{
    public class ExceptionLogDtoProfiles : Profile
    {
        public ExceptionLogDtoProfiles()
        {
            CreateMap<ExceptionLog, ExceptionLogDto>();
        }
    }
}