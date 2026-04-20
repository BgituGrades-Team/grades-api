using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Class;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Models;

namespace BgituGrades.Application.Mappings
{
    public class ClassProfile : Profile
    {
        public ClassProfile()
        {
            CreateMap<CreateClassRequest, ClassDTO>();
            CreateMap<GetClassDateRequest, ClassDateResponse>();
            CreateMap<ClassDateResponse, ScheduleDate>();
            CreateMap<Class, ClassResponse>();
            CreateMap<ClassDTO, ClassResponse>();
            CreateMap<Class, ClassDTO>();
            CreateMap<ClassDTO, Class>();
        }
    }
}
