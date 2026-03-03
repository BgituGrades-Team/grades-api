using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Discipline;

namespace BgituGrades.Mapping
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
