using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Auth;
using api.Dto.UserDto;
using api.Models;
using api.Response;

namespace api.Services.AuthService
{
    public interface IAuthService
    {
        Task<object> Register (AddUserDto model);
        Task<object> Login(LoginDto loginDto);
        Task<GenericResponse> ConfirmEmail(string tokenGuid, string otp);
        Task<bool> UserExists(string email);
        Task<bool> UserExistsTokenGuid(string tokenGuid);
        Task<string> GenerateOtpGuid(AppUser user);
        Task<bool> PhoneNumberExists(string PhoneNumber);

        Task<GenericResponse> UpdateUserPassword(string userName, string currentPassword, string newPassword);

    }
}