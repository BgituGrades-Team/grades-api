using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Group;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<CreateGroupRequest, Group>();
            CreateMap<UpdateGroupRequest, Group>();
            CreateMap<Group, GroupResponse>();
            CreateMap<Group, GroupDTO>();
            CreateMap<Group, CourseReponse>();
            CreateMap<Group, ArchivedGroupResponse>();
            CreateMap<GroupDTO, Group>();
        }
    }
}
