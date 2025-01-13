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
        public ICompanayRepositry companayRepositry { get; private set; }
        public IShoppingCartRepositry ShoppingCartRepositry { get; private set; }
        public IApplicationUserRepositry applicationUserRepositry { get; private set; }

        public IOrderDetailRepositry orderDetailRepositry { get; private set; }

        public IOrderHeaderRepositry orderHeaderRepositry { get; private set; }

        public UnitOfWork(ApplicationDbContext Db) 
        {
            _Db = Db;
            categoryRepositry = new CategoryRepositry(_Db);
            productRepositry = new ProductRepositry(_Db);
            companayRepositry = new CompanayRepositry(_Db);
            ShoppingCartRepositry = new ShoppingCartReposity(_Db);
            applicationUserRepositry = new ApplicationUserRepositry(_Db);
            orderHeaderRepositry = new OrderHeaderRepositry(_Db);
            orderDetailRepositry = new OrderDetailRepositry(_Db);
        }

        public void Save()
        {
            _Db.SaveChanges();
        }
    }
}
