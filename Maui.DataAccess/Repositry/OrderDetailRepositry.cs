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
    public class OrderDetailRepositry : Repositry<OrderDetail>, IOrderDetailRepositry
    {
        private readonly ApplicationDbContext _Db;
        public OrderDetailRepositry(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(OrderDetail entity)
        {
            _Db.orderDetails.Update(entity); 
        }
    }
}
