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
    public class ShoppingCartReposity : Repositry<ShoppingCart>, IShoppingCartRepositry
    {
        private readonly ApplicationDbContext _Db;
        public ShoppingCartReposity(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }

        public void Update(ShoppingCart entity)
        {
            _Db.ShoppingCarts.Update(entity);
        }
    }
}

