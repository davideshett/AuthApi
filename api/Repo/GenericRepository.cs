using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Params;
using api.Repo.Interface;
using api.Response;
using Microsoft.EntityFrameworkCore;

namespace api.Repo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext context;

        public GenericRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<object> AddAsync(T entity)
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            
            return new GenericResponse{
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 201
            };
        }

        public async Task<object> DeleteAsync(int id)
        {
            var data = await GetAsync(id);
            if (data == null)
            {
                return null;
            }

            context.Set<T>().Remove(data);
            await context.SaveChangesAsync();

            return new GenericResponse
            {
                Message = "Success",
                IsSuccessful = true,
                StatusCode = 204
            };
        }

        public async Task<bool> Exists(int id)
        {
           var data = await GetAsync(id);
           return data != null;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }


        public async Task<T> GetAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }
    }
}