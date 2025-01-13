using MauiBook.DataAccess.Data;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry
{
    public class CompanayRepositry : Repositry<Company>, ICompanayRepositry
    {
        private readonly ApplicationDbContext _Db;
        public CompanayRepositry(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(Company entity)
        {
            _Db.companies.Update(entity);
        }
    }
}
