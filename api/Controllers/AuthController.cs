using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Auth;
using api.Dto.UserDto;
using api.Models;
using api.Services.AuthService;
using api.Services.EmailService;
using api.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService authService;
        private readonly ITokenService tokenService;
        private readonly ILogger<AuthController> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;

        public AuthController(IAuthService authService,
        ITokenService tokenService, ILogger<AuthController> logger,
        UserManager<AppUser> userManager, IEmailService emailService,SignInManager<AppUser> signInManager)
        {
            this.authService = authService;
            this.tokenService = tokenService;
            this.logger = logger;
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] AddUserDto model)
        {
            logger.LogInformation($"Registration attempt for {model.Email}");

            try
            {
                var data = await authService.Register(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong in the {nameof(AddUserDto)}. Please contact support");
                return Problem($"Something went wrong in the {nameof(AddUserDto)}. Please contact support. {ex.Message}");
            }


        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var data = await authService.Login(model);
            return Ok(data);

        }

        [HttpPost("refreshtoken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] AuthResponseDto model)
        {
            var data = await tokenService.VerifyRefreshToken(model);

            if (data == null)
            {
                return Unauthorized();
            }
            return Ok(data);

        }

        [HttpPost("emailconfirmation/{TokenGuid}")]
        public async Task<IActionResult> ConfirmEmail(string TokenGuid, [FromBody] OtpDto model)
        {
            var data = await authService.ConfirmEmail(TokenGuid, model.Otp);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var dataFromRepo = await authService.UpdateUserPassword(model.UserName, model.CurrentPassword, model.NewPassword);
            return Ok(dataFromRepo);
        }

        [AllowAnonymous]
        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> SendPasswordResetLink([FromBody] UserEmailDto model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);

            if (existingUser == null)
            {
                return BadRequest(new
                {
                    IsSuccessful = false,
                    Message = "User not found",
                    StatusCode = 401
                });
            }

            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(existingUser);
            var resetLink = Url.Action
            (
                "ResetPassword",
                "Auth",
                values: new
                {
                    Email = existingUser.Email,
                    Token = passwordResetToken,
                }
            );
            var callbackUrl = Request.Scheme + "://" + Request.Host + resetLink + " " + "Click the link to reset password";

            var frontendPath = $"http://localhost:3000/auth-test/new-password/{model.Email}/{passwordResetToken}";
            var response = emailService.SendEmail(existingUser.Email, "PASSWORD RESET", frontendPath);
            return Ok(new
            {
                Url = callbackUrl,
                Token = passwordResetToken,
                //Data = response
            });
        }

        [HttpPost("resendotp/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new {
                    Message = "User not found",
                    IsSuccessful = false,
                    StatusCode = 404
                });
            }

            var code = new Random().Next(111111,999999).ToString();

            user.TempOtp = code;
            await userManager.UpdateAsync(user);

            var htmlMessage = $"<p>Hello {user.FirstName},<br>CODE: {code}<br> Follow the link to confirm email. http://localhost:5165/api/auth/emailconfirmation/{user.TokenGuid}</p>";
            var result = emailService.SendEmail(user.Email, "AUTH OTP", $"CODE: {htmlMessage}");
            
            return Ok();
        }

        [Authorize]
        [Route("logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
            }
            catch (System.InvalidOperationException ex)
            {
                 // TODO
                 return Ok(new {
                    Message = ex.Message
                 });
            }
           
            
            return Ok(new
            {
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 204
            });
        }



        [AllowAnonymous]
        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string Token, string Email, [FromBody] ResetPasswordDto model)
        {

            if (Token == null || Email == null)
            {
                return BadRequest(new
                {
                    Message = "invalid token or Email"
                });
            }

            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return BadRequest(new
                {
                    Message = "Invalid email parameters"
                });
            }


            var result = await userManager.ResetPasswordAsync(user, Token, model.NewPassword);
            await userManager.UpdateAsync(user);

            return Ok(result.Succeeded ? new
            {
                Message = $"Pasword reset for {user.Email} was successful. Proceed to login",
                StatusCode = 200,
                IsSuccessful = true,
            } : "Error");
        }

    }

    public class UserEmailDto
    {
        public string Email { get; set; }
    }

    public class OtpDto
    {
        public string Otp { get; set; }
    }
}
