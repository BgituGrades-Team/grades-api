using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Discipline;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class DisciplineProfile : Profile
    {
        public DisciplineProfile()
        {
            CreateMap<CreateDisciplineRequest, Discipline>();
            CreateMap<UpdateDisciplineRequest, Discipline>();
            CreateMap<Discipline, DisciplineResponse>();
            CreateMap<Discipline, DisciplineDTO>();
            CreateMap<DisciplineDTO, Discipline>();
        }
    }
}
