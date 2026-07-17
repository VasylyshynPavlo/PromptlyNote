using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Services.Mapping
{
    public class ToDoTaskProfile : Profile
    {
        public ToDoTaskProfile()
        {
            CreateMap<ToDoTask, ToDoTaskDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.TaskListId, opt => opt.MapFrom(src => src.TaskListId.ToString()))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId != null ? src.CategoryId.ToString() : null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));

            CreateMap<ToDoTaskDto, ToDoTask>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ParseToGuidWithThrow("todotask")))
                .ForMember(dest => dest.TaskListId, opt => opt.MapFrom(src => src.TaskListId.ParseToGuidWithThrow("tasklist")))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId != null ? src.CategoryId.ParseToGuidWithThrow("category") : (Guid?)null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ParseToGuidWithThrow("user")));

            CreateMap<ToDoTask, ToDoTaskLightDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.TaskListId, opt => opt.MapFrom(src => src.TaskListId.ToString()))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId != null ? src.CategoryId.ToString() : null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));
        }
    }
}
