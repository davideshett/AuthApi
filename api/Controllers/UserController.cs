using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto;
using api.Dto.UserDto;
using api.Helper;
using api.Models;
using api.Repo.Interface;
using api.Services.PhotoService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class UserController: BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;
        private readonly IPhotoService photoService;
        private readonly DataContext dataContext;

        public UserController(IUserRepository userRepository, IMapper mapper,
        UserManager<AppUser> userManager, IPhotoService photoService, DataContext dataContext)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.userManager = userManager;
            this.photoService = photoService;
            this.dataContext = dataContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers()
        {
            var data = await userRepository.GetAllUsers();
            
            return Ok(new{
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 200,
                Data = mapper.Map<ICollection<GetListOfUsers>> (data)
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var data = await userRepository.GetUserById(id);

            return Ok(new
            {
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 200,
                Data = mapper.Map<GetUserById>(data)
            });
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersPaged([FromQuery] UserParams userParams)
        {
            var dataFromRepo = await userRepository.GetUsersPaged(userParams);

            Response.AddPagination(dataFromRepo.CurrentPage, dataFromRepo.PageSize,
                 dataFromRepo.TotalCount, dataFromRepo.TotalPages);

            return Ok(new
            {
                dataFromRepo.CurrentPage,
                dataFromRepo.PageSize,
                dataFromRepo.TotalCount,
                dataFromRepo.TotalPages,
                Message = "Success",
                StatusCode = 201,
                IsSuccessful = true,
                Data = mapper.Map<ICollection<GetListOfUsers>>(dataFromRepo) 
            });
        }


        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProflile([FromForm] UpdateProfileDto model)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue("uid"));
            var dataFromRepo = await userRepository.UpdateUser(userId, model);
            return Ok(dataFromRepo);
        }

        [HttpPost("adduserstorole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddusersToRole(AddUserToRoleDto model)
        {
            var dataFromRepo = await userRepository.AddUsersToRole(model);
            return Ok(dataFromRepo);
        }

        [HttpPost("removeusersfromrole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUsersFromRole(RemoveUsersFromRole model)
        {
            var dataFromRepo = await userRepository.RemoveUsersFromRole(model);
            return Ok(dataFromRepo);
        }

        [HttpDelete("delete/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUsers(string email)
        {
            var dataFromRepo = await userRepository.DeleteUser(email);
            return dataFromRepo == null ? 
            NotFound(new {
                Message = "User not found",
                IsSuccessful = false,
                StatusCode = 404
            }) 
            : Ok(dataFromRepo);
        }
    }
}