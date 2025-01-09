using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IShoppingCartRepositry : IRepositry<ShoppingCart>
    {
        public void Update(ShoppingCart entity);
    }
}
