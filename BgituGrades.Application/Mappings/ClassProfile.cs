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
            CreateMap<CreateClassRequest, Class>();
            CreateMap<GetClassDateRequest, ClassDateResponse>();
            CreateMap<ClassDateResponse, ScheduleDate>();
            CreateMap<Class, ClassResponse>();
            CreateMap<Class, ClassDTO>();
            CreateMap<ClassDTO, Class>();
        }
    }
}
