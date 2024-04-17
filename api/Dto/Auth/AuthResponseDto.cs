using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}