using AutoMapper;
using BgituGrades.Application.Models.Key;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class KeyProfile : Profile
    {
        public KeyProfile()
        {
            CreateMap<ApiKey, KeyResponse>();
        }
    }
}
