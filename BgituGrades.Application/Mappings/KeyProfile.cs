using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Key;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class KeyProfile : Profile
    {
        public KeyProfile()
        {
            CreateMap<KeyDTO, KeyResponse>();
            CreateMap<ApiKey, KeyDTO>();
        }
    }
}