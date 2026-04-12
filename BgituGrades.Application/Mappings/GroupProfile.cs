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
            CreateMap<CreateGroupRequest, GroupDTO>();
            CreateMap<UpdateGroupRequest, GroupDTO>();
            CreateMap<GroupDTO, Group>();
            CreateMap<Group, GroupDTO>();
            CreateMap<GroupDTO, GroupResponse>();
            CreateMap<GroupDTO, ArchivedGroupResponse>();
        }
    }
}
