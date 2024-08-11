using AutoMapper;
using ConfigurationLib.Dashboard.Models.Inputs;
using ConfigurationLib.Models;
using ConfigurationLib.Models.Dtos;
using MongoDB.Bson;
using System;

namespace ConfigurationLib.Dashboard.Profiles
{
    public class ApplicationConfigurationProfile : Profile
    {
        public ApplicationConfigurationProfile()
        {
            CreateMap<ApplicationConfigurationDto, ApplicationConfiguration>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => new ObjectId(s.Id)));
        }
    }
}