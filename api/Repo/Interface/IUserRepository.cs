using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;
using api.Dto.UserDto;
using api.Helper;
using api.Models;

namespace api.Repo.Interface
{
    public interface IUserRepository: IGenericRepository<AppUser>
    {
        Task<ICollection<AppUser>> GetAllUsers();
        Task<PagedList<AppUser>> GetUsersPaged(UserParams userParams);
        Task<object> GetUserById(int id);
        Task<object> DeleteUser(string email); 
        Task<object> AddUsersToRole(AddUserToRoleDto userToRoleDto);
        Task<object> RemoveUsersFromRole(RemoveUsersFromRole removeUsersFromRole);
        Task<object> UpdateUser(int id, UpdateProfileDto model);
    }
}