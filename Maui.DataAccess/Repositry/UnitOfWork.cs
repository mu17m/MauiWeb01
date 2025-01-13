using MauiBook.DataAccess.Data;
using MauiBook.DataAccess.Repositry.IRepositry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _Db;
        public ICategoryRepositry category { get; private set; }
        
        public UnitOfWork(ApplicationDbContext Db)
        {
            _Db = Db;
            category = new CategoryRepositry(_Db);
        }

        public void Save()
        {
            _Db.SaveChanges();
        }
    }
}
