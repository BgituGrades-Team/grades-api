using AutoMapper;
using BgituGrades.Application.Models.Setting;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class SettingProfile : Profile
    {
        public SettingProfile()
        {
            CreateMap<UpdateSettingRequest, Setting>();
            CreateMap<Setting, SettingResponse>();
        }
    }
}
