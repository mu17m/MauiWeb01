using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface ICategoryRepositry : IRepositry<Category>
    {
        public void Update(Category entity);
    }
}
