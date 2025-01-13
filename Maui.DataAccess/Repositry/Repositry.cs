using MauiBook.DataAccess.Repositry.IRepositry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiBook.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace MauiBook.DataAccess.Repositry
{
    public class Repositry<T> : IRepositry<T> where T : class
    {
        private readonly ApplicationDbContext _Db;
        private DbSet<T> dbset;

        public Repositry(ApplicationDbContext Db)
        {
            _Db = Db;
            this.dbset = _Db.Set<T>();
            _Db.products.Include(u => u.category).Include(c => c.CategoryId);
        }

        public void Add(T entity)
        {
            dbset.Add(entity);
        }
        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperity = null, bool tracked = false)
        {
            IQueryable<T> query;
            if(tracked)
            {
                query = dbset;
            }   
            else
            {
                query = dbset.AsNoTracking();
            }
            query = query.Where(filter);
            if(!string.IsNullOrEmpty(includeProperity))
            {
                foreach(var prop in includeProperity.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return query.FirstOrDefault();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? includeProperities = null)
        {
            IQueryable<T> query = dbset;
            if(filter != null)
            {
                query = query.Where(filter);
            }
            if(!string.IsNullOrEmpty(includeProperities))
            {
                foreach(var properity in includeProperities.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(properity);
                }
            }
            return query.ToList();
        }
        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }
        public void RemoveRange(List<T> entity)
        {
            dbset.RemoveRange(entity);
        }
    }
}
