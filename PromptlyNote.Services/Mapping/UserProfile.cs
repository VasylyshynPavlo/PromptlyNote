using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;

namespace PromptlyNote.Services.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(
                    dest => dest.IsPasswordSet,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.PasswordHash)));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
        }
    }
}
