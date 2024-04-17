using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Params;
using api.Helper;
using api.Models;
using api.Repo.Interface;

namespace api.Repo
{
    public class RoleRepository : GenericRepository<AppRole>, IRoleRepository
    {
        private readonly DataContext context;

        public RoleRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<PagedList<AppRole>> GetRoles(string name, BaseParams baseParams)
        {
            var data = context.Roles.AsQueryable();

            if(! string.IsNullOrEmpty(name))
            {
                data = data.Where(x=> x.Name.ToLower().Contains(name.ToLower()));
            }

            return await PagedList<AppRole>.CreateAsync(data,baseParams.PageNumber,baseParams.PageSize);
        }
    }
}