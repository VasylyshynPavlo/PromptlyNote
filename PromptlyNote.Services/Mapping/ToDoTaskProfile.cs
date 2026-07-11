using AutoMapper;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.TaskListId, opt => opt.MapFrom(src => Guid.Parse(src.TaskListId)))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId != null ? Guid.Parse(src.CategoryId) : (Guid?)null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)));

            CreateMap<ToDoTask, ToDoTaskLightDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.TaskListId, opt => opt.MapFrom(src => src.TaskListId.ToString()))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId != null ? src.CategoryId.ToString() : null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));
        }
    }
}
