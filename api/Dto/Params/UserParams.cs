using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Params;
using Microsoft.AspNetCore.Mvc;

namespace api.Dto.UserDto
{
    public class UserParams: BaseParams
    {
        [FromHeader]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}