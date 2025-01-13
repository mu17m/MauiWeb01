using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IProductRepositry : IRepositry<Product>
    {
        public void Update(Product entity);
    }
}
