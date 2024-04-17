using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Auth;
using api.Models;

namespace api.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
        Task<string> CreateRefreshToken(AppUser user);
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
    }
}