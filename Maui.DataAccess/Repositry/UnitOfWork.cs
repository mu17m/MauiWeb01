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
        private readonly ApplicationDbContext _Db;
        public ICategoryRepositry categoryRepositry {  get; private set; }
        public IProductRepositry productRepositry { get; private set; }

        public UnitOfWork(ApplicationDbContext Db) 
        {
            _Db = Db;
            categoryRepositry = new CategoryRepositry(Db);
            productRepositry = new ProductRepositry(Db);
        }

        public void Save()
        {
            _Db.SaveChanges();
        }
    }
}
