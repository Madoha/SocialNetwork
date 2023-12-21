using AutoMapper;
using SocialNetwork.Contracts.Authentication;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<RegisterRequest, ApplicationUser>().ReverseMap();
            CreateMap<RegisterResponse, ApplicationUser>().ReverseMap();
            CreateMap<LoginRequest, ApplicationUser>().ReverseMap();
        }
    }
}
