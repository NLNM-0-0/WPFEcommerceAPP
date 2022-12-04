using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IGenericDataRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(params Expression<Func<T, object>>[] navigationProperties);
        Task<IList<T>> GetListAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        Task<T> GetSingleAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        Task Add(params T[] items);
        Task Update(params T[] items);
        Task Remove(params T[] items);
    }
}
