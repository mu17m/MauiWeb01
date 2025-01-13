using MauiBook.DataAccess.Data;
using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IApplicationUserRepositry : IRepositry<ApplicationUser>
    {
        public void Update(ApplicationUser user);
    }
}
