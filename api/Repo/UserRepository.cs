using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto;
using api.Dto.UserDto;
using api.Helper;
using api.Models;
using api.Repo.Interface;
using api.Response;
using api.Services.PhotoService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repo
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly DataContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPhotoService photoService;

        public UserRepository(DataContext context, UserManager<AppUser> userManager, 
        RoleManager<AppRole> roleManager, IHttpContextAccessor httpContextAccessor, IPhotoService photoService) : base(context)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.photoService = photoService;
        }

        public async Task<ICollection<AppUser>> GetAllUsers()
        {
            var data = await context.Users.Include(x=> x.UserRoles)
            .ThenInclude(x=> x.Role)
            .ToListAsync();
            return data;
        }

        public async Task<object> GetUserById(int id)
        {
            var data = await context.Users
            .Include(x=> x.LatestActivities)
            .Include(x => x.UserRoles)
            .ThenInclude(x=> x.Role)
            .FirstOrDefaultAsync(x=> x.Id==id);
            if(data == null)
            {
                return new GenericResponse
                {
                    Message = "User not found",
                    IsSuccessful = false,
                    StatusCode = 400
                };
            }
            return data;
        }

        public async Task<object> UpdateUser(int userId, UpdateUser model)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x=> x.Id == userId);
            if (user == null)
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

            if(result.Succeeded)
            {
                var latActivity = new LatestActivity
                {
                    Activity = "Updated user profile",
                    Timestamp = DateTime.Now.ToString("f", CultureInfo.CreateSpecificCulture("en-US")),
                    UserId = user.Id
                };
                await context.LatestActivities.AddAsync(latActivity);
                return user;
            }

            return new GenericResponse
            {
                Message = "Error",
                IsSuccessful = false,
                StatusCode = 401
            };
            
        }

        public async Task<PagedList<AppUser>> GetUsersPaged(UserParams userParams)
        {
            var data = context.Users.Include(x=> x.UserRoles)
            .ThenInclude(x=> x.Role)
            .AsQueryable();

            if(userParams.Id > 0)
            {
                data = data.Where(x=> x.Id == userParams.Id);
            }

            if(!string.IsNullOrEmpty(userParams.Email))
            {
                data = data.Where(x=> x.Email.ToLower().Contains(userParams.Email.ToLower()));
            }

            if (!string.IsNullOrEmpty(userParams.FirstName))
            {
                data = data.Where(x => x.FirstName.ToLower().Contains(userParams.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(userParams.LastName))
            {
                data = data.Where(x => x.LastName.ToLower().Contains(userParams.LastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(userParams.PhoneNumber))
            {
                data = data.Where(x => x.PhoneNumber.ToLower().Contains(userParams.PhoneNumber.ToLower()));
            }

            if (!string.IsNullOrEmpty(userParams.Role))
            {
                data = data.Where(x => x.UserRoles.Select(x=> x.Role.Name).Contains(userParams.Role));
            }

            return await PagedList<AppUser>.CreateAsync(data,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<object> AddUsersToRole(AddUserToRoleDto userToRoleDto)
        {
            foreach(var user in userToRoleDto.UserId)
            {
                if (! await Exists(user))
                {
                    return new GenericResponse
                    {
                        Message = $"UserID {user} does not exist",
                        IsSuccessful = false,
                        StatusCode = 401,
                    };
                }
            }

           
            if (await roleManager.RoleExistsAsync(userToRoleDto.Role))
            {
                foreach (var user in userToRoleDto.UserId)
                {
                    var existingUser = await GetAsync(user);
                    await userManager.AddToRoleAsync(existingUser, userToRoleDto.Role);
                    await userManager.UpdateAsync(existingUser);
                }

                return new GenericResponse
                {
                    Message = "Success",
                    IsSuccessful = true,
                    StatusCode = 201,
                };

            }

            return new GenericResponse
            {
                Message = "Role does not exist",
                IsSuccessful = false,
                StatusCode = 401,
            };



        }

        public async Task<object> RemoveUsersFromRole(RemoveUsersFromRole removeUsersFromRole)
        {
            foreach (var user in removeUsersFromRole.UserId)
            {
                if (!await Exists(user))
                {
                    return new GenericResponse
                    {
                        Message = $"UserID {user} does not exist",
                        IsSuccessful = false,
                        StatusCode = 401,
                    };
                }
            }


            if (await roleManager.RoleExistsAsync(removeUsersFromRole.RoleName))
            {
                foreach (var user in removeUsersFromRole.UserId)
                {
                    var existingUser = await GetAsync(user);
                    await userManager.RemoveFromRoleAsync(existingUser, removeUsersFromRole.RoleName);
                    await userManager.UpdateAsync(existingUser);
                }

                return new GenericResponse
                {
                    Message = "Success",
                    IsSuccessful = true,
                    StatusCode = 201,
                };


            }

            return new GenericResponse
            {
                Message = "Role does not exist",
                IsSuccessful = false,
                StatusCode = 401,
            };
        }

        public async Task<object> DeleteUser(string email)
        {
            var data = await userManager.FindByEmailAsync(email);
            if(data == null)
            {
                // return new GenericResponse
                // {
                //     Message = "User not found",
                //     IsSuccessful = false,
                //     StatusCode = 404
                // };
                return null;
            }

            await userManager.DeleteAsync(data);
            return new GenericResponse
            {
                Message = "User deleted successfully",
                IsSuccessful = true,
                StatusCode = 200
            };
        }

        

       

        public async Task<object> UpdateUser(int id, UpdateProfileDto model)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            var avi = user.ProfilePicture;
            if (model.ProfilePicture != null)
            {
                var data = await photoService.AddPhotoAsync(model.ProfilePicture);
                avi = data.Url.ToString();
            }


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.ProfilePicture = avi;

            var result = await userManager.UpdateAsync(user);
            await context.SaveChangesAsync();

            if (result.Succeeded)
            {
                var latActivity = new LatestActivity
                {
                    Activity = "Updated user profile",
                    Timestamp = DateTime.Now.ToString("f", CultureInfo.CreateSpecificCulture("en-US")),
                    UserId = user.Id
                };
                await context.LatestActivities.AddAsync(latActivity);
                await context.SaveChangesAsync();
            }

            return new GenericResponse
            {
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 201
            };

        }
    }
}