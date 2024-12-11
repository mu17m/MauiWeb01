using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.DataAccess.Data;
namespace MauiBook.DataAccess.Repositry
{
    public class CategoryRepositry : Repositry<Category>, ICategoryRepositry
    {
        private readonly ApplicationDbContext _Db;
        public CategoryRepositry(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(Category entity)
        {
            _Db.categories.Update(entity);
        }
    }
}

