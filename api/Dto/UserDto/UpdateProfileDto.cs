using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.UserDto
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}