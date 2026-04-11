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
            CreateMap<CreateWorkRequest, Work>();
            CreateMap<UpdateWorkRequest, Work>();
            CreateMap<Work, WorkResponse>();
            CreateMap<Work, WorkDTO>();
            CreateMap<WorkDTO, Work>();
        }
    }
}
