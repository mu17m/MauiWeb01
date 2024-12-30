using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiBook.Models;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface ICompanayRepositry : IRepositry<Company>
    {
        public void Update(Company entity);
    }
}
