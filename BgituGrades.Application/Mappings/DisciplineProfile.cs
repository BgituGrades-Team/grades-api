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
            CreateMap<CreateDisciplineRequest, DisciplineDTO>();
            CreateMap<UpdateDisciplineRequest, DisciplineDTO>();
            CreateMap<DisciplineDTO, Discipline>();
            CreateMap<Discipline, DisciplineDTO>();
            CreateMap<DisciplineDTO, DisciplineResponse>();
        }
    }
}
