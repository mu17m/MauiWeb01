using MauiBook.DataAccess.Data;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry
{
    public class ApplicationUserRepositry : Repositry<ApplicationUser>, IApplicationUserRepositry
    {
        private readonly ApplicationDbContext _Db;
        public ApplicationUserRepositry(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(ApplicationUser user)
        {
            _Db.ApplicationUsers.Update(user);
        }
    }
}
