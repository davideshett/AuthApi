using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Dto.Auth;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext dataContext;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager, DataContext dataContext)
        {
            this.config = config;
            _userManager = userManager;
            this.dataContext = dataContext;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JwtSettings:Key").Value));
        }

        public async Task<string> CreateToken(AppUser user)
        {

            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new ("uid", user.Id.ToString()),
            }
            .Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: config["JwtSettings:Issuer"],
                audience: config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(config["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshToken(AppUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, config["TokenSettings:LoginProvider"], config["TokenSettings:RefreshToken"]);

            var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, config["TokenSettings:LoginProvider"], config["TokenSettings:RefreshToken"]);
            var result = await _userManager.SetAuthenticationTokenAsync
            (
                user,
                config["TokenSettings:LoginProvider"],
                config["TokenSettings:RefreshToken"],
                newRefreshToken
            );

            return newRefreshToken;
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);

            var UserName = tokenContent.Claims.ToList().FirstOrDefault(q=> q.Type == 
            JwtRegisteredClaimNames.Email)?.Value;

            var user = await _userManager.FindByNameAsync(UserName);

            if(user == null)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync
            (
                user, 
                config["TokenSettings:LoginProvider"], 
                config["TokenSettings:RefreshToken"], 
                request.RefreshToken
            );

            if(isValidRefreshToken)
            {
                var token = await CreateToken(user);
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    RefreshToken = await CreateRefreshToken(user)
                };
            }

            await _userManager.UpdateSecurityStampAsync(user);
            return null;
        }
    }
}