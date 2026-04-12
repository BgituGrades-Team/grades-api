using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Work;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class WorkProfile : Profile
    {
        public WorkProfile()
        {
            CreateMap<CreateWorkRequest, WorkDTO>();
            CreateMap<UpdateWorkRequest, WorkDTO>();
            CreateMap<WorkDTO, Work>();
            CreateMap<Work, WorkDTO>();
            CreateMap<WorkDTO, WorkResponse>();
        }
    }
}
