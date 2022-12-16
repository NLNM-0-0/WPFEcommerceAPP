using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using WPFEcommerceApp.Models;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Documents;

namespace DataAccessLayer
{
    public class GenericDataRepository<T> : IGenericDataRepository<T> where T : class
    {
        // Add
        public virtual async Task Add(params T[] items)
        {
            using (var context = new EcommerceAppEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = EntityState.Added;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<IList<T>> GetAllAsync(params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list=new List<T>();
            await Task.Run(() =>
            {
                using (var context = new EcommerceAppEntities())
                {
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    list = dbQuery
                        .AsNoTracking()
                        .ToList<T>();
                    
                }
            });
            return list;

        }


        public async Task<IList<T>> GetListAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list = new List<T>();
            await Task.Run(() =>
            {
                using (var context = new EcommerceAppEntities())
                {
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    list = dbQuery
                        .AsNoTracking()
                        .Where(where)
                        .ToList<T>();
                }
            });
            return list;
        }


        public async Task<T> GetSingleAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            await Task.Run(() =>
            {
                using (var context = new EcommerceAppEntities())
                {
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    item = dbQuery
                        .AsNoTracking() //Don't track any changes for the selected item
                        .FirstOrDefault(where); //Apply where clause
                }
            });
            return item;
        }

        public async Task Remove(params T[] items)
        {

            using (var context = new EcommerceAppEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = EntityState.Deleted;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task Update(params T[] items)
        {
            using (var context = new EcommerceAppEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
        }

    }
}
