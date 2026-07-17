using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Services.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));

            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ParseToGuidWithThrow("category")))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ParseToGuidWithThrow("user")));
        }
    }
}
