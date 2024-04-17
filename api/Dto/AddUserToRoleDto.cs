using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class AddUserToRoleDto
    {
        public List<int> UserId { get; set; }
        public string Role { get; set; }
    }
}