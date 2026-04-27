using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Class;
using BgituGrades.Application.Models.Mark;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Models;

namespace BgituGrades.Application.Mappings
{
    public class MarkProfile : Profile
    {
        public MarkProfile()
        {
            CreateMap<CreateMarkRequest, Mark>();
            CreateMap<UpdateMarkRequest, Mark>();
            CreateMap<UpdateMarkRequest, MarkResponse>();
            CreateMap<UpdateMarkGradeRequest, Mark>();
            CreateMap<Mark, MarkResponse>();
            CreateMap<Mark, MarkDTO>();
            CreateMap<MarkDTO, Mark>();
            CreateMap<StudentMarkResult, FullGradeMarkResponse>();
            CreateMap<MarkEntry, GradeMarkResponse>();
        }
    }
}
