using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Params;
using api.Dto.Role;
using api.Helper;
using api.Models;
using api.Repo.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    public class RoleController: BaseController
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        private readonly RoleManager<AppRole> roleManager;

        public RoleController(IRoleRepository roleRepository, IMapper mapper, RoleManager<AppRole> roleManager)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddRole (AddRoleDto model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(await roleManager.RoleExistsAsync(model.Name))
            {
                return BadRequest(new {
                    Message = "Role exists already",
                    IsSuccessful = false,
                    StatusCode = 401
                });
            }

            var role = mapper.Map<AppRole>(model);
            var result = await roleRepository.AddAsync(role);

            return Ok(new {
                Message = "Role added successfully",
                IsSuccessful = true,
                StatusCode = 201
            });

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRoles(string name, [FromQuery] BaseParams baseParams)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var data = await roleRepository.GetRoles(name,baseParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            return Ok(new
            {
                data.CurrentPage,
                data.PageSize,
                data.TotalCount,
                data.TotalPages,
                Message = "Success",
                StatusCode = 200,
                IsSuccessful = true,
                Data = mapper.Map<ICollection<GetRoleDto>>(data)
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRole(int id)
        {
            var data = await roleRepository.GetAsync(id);


            if(data == null)
            {
                return BadRequest(new {
                    Message = "Role does not exist",
                    IsSuccessful = false,
                    StatusCode = 401
                });
            }

            return Ok(new
            {
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 201,
                Data = mapper.Map<GetRoleDto>(data)
            });

        }

        [HttpPut("update/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateRole(int id, AddRoleDto model)
        {
            var role = await roleRepository.GetAsync(id);
            if (role == null)
            {
                return BadRequest(new
                {
                    Message = "Role does not exist",
                    IsSuccessful = false,
                    StatusCode = 401
                });
            }

            role.Name = model.Name;

            await roleRepository.UpdateAsync(role);

            return Ok(new {
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 201
            });
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var data = await roleRepository.GetAsync(id);


            if (data == null)
            {
                return BadRequest(new
                {
                    Message = "Role does not exist",
                    IsSuccessful = false,
                    StatusCode = 401
                });
            }
            return Ok(await roleRepository.DeleteAsync(id));
        }
    }
}