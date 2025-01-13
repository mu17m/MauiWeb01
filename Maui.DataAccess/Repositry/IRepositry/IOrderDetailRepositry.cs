using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IOrderDetailRepositry : IRepositry<OrderDetail>
    {
        public void Update(OrderDetail entity);
    }
}
