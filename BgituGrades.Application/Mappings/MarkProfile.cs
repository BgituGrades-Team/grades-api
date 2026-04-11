using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Mark;
using BgituGrades.Domain.Entities;

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
        }
    }
}
