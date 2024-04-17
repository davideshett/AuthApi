using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Params;
using api.Dto.Role;
using api.Helper;
using api.Models;

namespace api.Repo.Interface
{
    public interface IRoleRepository : IGenericRepository<AppRole>
    {
        Task<PagedList<AppRole>> GetRoles(string name, BaseParams baseParams);
    }
}