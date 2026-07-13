using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Services.Mapping
{
    public class SubTaskProfile : Profile
    {
        public SubTaskProfile()
        {
            CreateMap<CreateSubTaskForm, SubTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<SubTask, SubTaskDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<SubTaskDto, SubTask>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ParseToGuidWithThrow("sub task")));

            CreateMap<UpdateSubTaskForm, SubTask>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ParseToGuidWithThrow("sub task")));
        }
    }
}
