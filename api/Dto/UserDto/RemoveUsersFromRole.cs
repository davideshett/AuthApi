using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.UserDto
{
    public class RemoveUsersFromRole
    {
        public List<int> UserId { get; set; }
        public string RoleName { get; set; }
    }
}