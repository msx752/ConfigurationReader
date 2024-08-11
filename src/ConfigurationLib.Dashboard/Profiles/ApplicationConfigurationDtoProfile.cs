using AutoMapper;
using ConfigurationLib.Models;
using ConfigurationLib.Models.Dtos;

namespace ConfigurationLib.Dashboard.Profiles
{
    public class ApplicationConfigurationDtoProfile : Profile
    {
        public ApplicationConfigurationDtoProfile()
        {
            CreateMap<ApplicationConfiguration, ApplicationConfigurationDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));
        }
    }
}