using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Role;
using api.Models;

namespace api.Dto.UserDto
{
    public class GetUserById
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; }
        public int LoginCount { get; set; }
        public ICollection<RoleNameDto> RoleForReturn { get; set; }
        public ICollection<LatestActivityStripped> LatestActivities { get; set; }
    }
}