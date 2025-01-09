using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IUnitOfWork
    {
        ICategoryRepositry categoryRepositry { get; }
        IProductRepositry productRepositry { get; }
        ICompanayRepositry companayRepositry { get; }
        IShoppingCartRepositry ShoppingCartRepositry { get; }
        IApplicationUserRepositry applicationUserRepositry { get; }
        IOrderDetailRepositry orderDetailRepositry { get; }
        IOrderHeaderRepositry orderHeaderRepositry { get; }
        void Save();
    }
}
