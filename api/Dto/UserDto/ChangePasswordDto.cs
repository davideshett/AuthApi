using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.UserDto
{
    public class ChangePasswordDto
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}