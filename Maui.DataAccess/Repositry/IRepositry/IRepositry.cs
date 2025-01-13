using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IRepositry<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? includeProperity = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperity = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(List<T> entity);
    }
}
