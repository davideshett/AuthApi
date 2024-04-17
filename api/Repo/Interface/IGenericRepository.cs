using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repo.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<ICollection<T>> GetAllAsync();
        Task<object> AddAsync(T entity);
        Task<object> DeleteAsync(int id);
        Task UpdateAsync(T entity);
        Task<bool> Exists(int id);
    }
}