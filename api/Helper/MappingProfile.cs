using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Role;
using api.Dto.UserDto;
using api.Models;
using AutoMapper;

namespace api.Helper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AddUserDto>().ReverseMap();
            CreateMap<AppRole, GetRoleDto>().ReverseMap();
            CreateMap<AppRole, AddRoleDto>().ReverseMap();
            CreateMap<AppRole, RoleNameDto>().ReverseMap();
            CreateMap<AppUser, GetUserById>().ReverseMap();
            CreateMap<LatestActivity, LatestActivityStripped>().ReverseMap();

            CreateMap<AppUser, GetListOfUsers>()
            .ForMember(dto => dto.RoleForReturn, c => c.MapFrom(c => c.UserRoles
            .Select(cs => cs.Role))).ReverseMap();
        }
    }
}