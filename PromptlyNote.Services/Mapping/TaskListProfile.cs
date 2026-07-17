using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Services.Mapping
{
    public class TaskListProfile : Profile
    {
        public TaskListProfile()
        {
            CreateMap<TaskList, TaskListDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));

            CreateMap<TaskListDto, TaskList>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ParseToGuidWithThrow("tasklist")))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ParseToGuidWithThrow("user")));
        }
    }
}
