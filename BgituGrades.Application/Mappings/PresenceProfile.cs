using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Class;
using BgituGrades.Application.Models.Presence;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Models;

namespace BgituGrades.Application.Mappings
{
    public class PresenceProfile : Profile
    {
        public PresenceProfile()
        {
            CreateMap<CreatePresenceRequest, Presence>();
            CreateMap<UpdatePresenceGradeRequest, Presence>();
            CreateMap<Presence, PresenceResponse>();
            CreateMap<Presence, PresenceDTO>();
            CreateMap<PresenceDTO, Presence>();
            CreateMap<StudentPresenceResult, FullGradePresenceResponse>();
            CreateMap<PresenceEntry, GradePresenceResponse>();
        }
    }
}
