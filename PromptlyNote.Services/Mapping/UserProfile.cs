using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Services.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
        }
    }
}
