using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Auth;
using api.Dto.UserDto;
using api.Models;
using api.Response;
using api.Services.EmailService;
using api.Services.TokenService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly ITokenService tokenService;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly DataContext context;

        public AuthService(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, 
        RoleManager<AppRole> roleManager, IConfiguration configuration, IEmailService emailService,
        DataContext context)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.emailService = emailService;
            this.context = context;
        }

       

        public async Task<object> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            bool isValidUser = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if(await userManager.IsEmailConfirmedAsync(user) && isValidUser)
            {
                var token = await tokenService.CreateToken(user);
                
                user.LoginCount +=1;
                await userManager.UpdateAsync(user);

                var latActivity = new LatestActivity
                {
                    Activity = "Successful login",
                    Timestamp = DateTime.Now.ToString("f", CultureInfo.CreateSpecificCulture("en-US")),
                    UserId = user.Id
                };

                await context.LatestActivities.AddAsync(latActivity);
                await context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Token = token,
                    RefreshToken = await tokenService.CreateRefreshToken(user)
                };

            }

            if(user == null || isValidUser == false)
            {
                return new GenericResponse
                {
                    Message = "Email or password not correct",
                    IsSuccessful = false,
                    StatusCode = 401
                };
            }

            return new GenericResponse
            {
                Message = "Email not confirmed",
                IsSuccessful = false,
                StatusCode = 401
            };

            
            
        }

        public async Task<object> Register(AddUserDto model)
        {
            if(await UserExists(model.Email) || await PhoneNumberExists(model.PhoneNumber))
            {
                return new GenericResponse
                {
                    Message = "Email or phone number already exists",
                    IsSuccessful = false,
                    StatusCode = 401
                };
            }
            var code = Convert.ToString(RandomNumber(111111, 999999));
            var otpGuid = Guid.NewGuid().ToString();

            var user = mapper.Map<AppUser>(model);
            user.UserName = model.Email;
            user.TempOtp = code;
            user.TokenGuid = otpGuid;

            var result = await userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {

                await userManager.AddToRoleAsync(user,"User");

                var htmlMessage = $"<p>Hello {user.FirstName},<br>CODE: {code}<br> Follow the link to confirm email. http://localhost:3000/auth-test/confirm-email/{user.TokenGuid}</p>";
                var data = emailService.SendEmail(user.Email, "AUTH OTP", $"CODE: {htmlMessage}");

                return new GenericResponse
                {
                    Message = "Success",
                    IsSuccessful = true,
                    StatusCode = 201
                };
            }

            return result.Errors;
        }

       

        public int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        public async Task<string> GenerateOtpGuid(AppUser user)
        {
            var otpGuid = Guid.NewGuid().ToString();
            user.TokenGuid = otpGuid;
            await userManager.UpdateAsync(user);
            return otpGuid;
        }

        public async Task<bool> UserExists(string email)
        {
            return await userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        public async Task<bool> UserExistsTokenGuid(string tokenGuid)
        {
            return await userManager.Users.AnyAsync(x => x.TokenGuid == tokenGuid.ToLower());
        }

        public async Task<bool> PhoneNumberExists(string PhoneNumber)
        {
            return await userManager.Users.AnyAsync(x => x.PhoneNumber == PhoneNumber);
        }

        public async Task<object> UpdateUser(string email, UpdateUser model)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return new GenericResponse
                {
                    Message = "User not found"
                };
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            return !result.Succeeded ? null : user;
        }

        public async Task<GenericResponse> UpdateUserPassword(string userName, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return new GenericResponse
                {
                    Message = "User not found",
                    IsSuccessful = false,
                    StatusCode = 401
                };
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                await userManager.UpdateAsync(user);
                return new GenericResponse
                {
                    Message = "Password changed successfully",
                    IsSuccessful = true,
                    StatusCode = 201
                };
            }

            return new GenericResponse
            {
                Message = string.Join(",", result.Errors),
                IsSuccessful = false,
                StatusCode = 401
            };
        }

        public async Task<GenericResponse> ConfirmEmail(string tokenGuid, string otp)
        {
            if (! await UserExistsTokenGuid(tokenGuid))
            {
                return new GenericResponse
                {
                    Message = "Invalid token",
                    IsSuccessful = false,
                    StatusCode = 401
                };
            }

            if (await UserExistsTokenGuid(tokenGuid))
            {
                var user = await userManager.Users.FirstOrDefaultAsync(x=> x.TokenGuid == tokenGuid);
                if(user.TempOtp == otp)
                {
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    return new GenericResponse
                    {
                        Message = "Email confirmed successfully",
                        IsSuccessful = true,
                        StatusCode = 200
                    };
                }
            }

            return new GenericResponse
            {
                Message = "Email confirmation failed",
                IsSuccessful = false,
                StatusCode = 401
            };
        }

    }
}