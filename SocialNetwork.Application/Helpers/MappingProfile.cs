using AutoMapper;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.Authentication;
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
            CreateMap<Post, AddPost>().ReverseMap();
            CreateMap<Post, EditPost>().ReverseMap();
            CreateMap<UserDTO, ApplicationUser>().ReverseMap();
            CreateMap<PostDTO, Post>().ReverseMap();
            CreateMap<CommentDTO, Comment>().ReverseMap();
        }
    }
}
