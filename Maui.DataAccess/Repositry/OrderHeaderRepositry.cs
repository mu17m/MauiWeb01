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
    public class OrderHeaderRepositry : Repositry<OrderHeader>, IOrderHeaderRepositry
    {
        private readonly ApplicationDbContext _Db;
        public OrderHeaderRepositry(ApplicationDbContext Db) : base(Db)
        {
            _Db = Db;
        }
        public void Update(OrderHeader entity)
        {
            _Db.orderHeaders.Update(entity); 
        }

        public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus)
        {
            var OrderFromDb = _Db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if(OrderFromDb != null)
            {
                OrderFromDb.OrderStatus = OrderStatus;
                if(!String.IsNullOrEmpty(PaymentStatus))
                {
                    OrderFromDb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string SessionId, string PaymentIntentId)
        {
            var OrderFromDb = _Db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if(OrderFromDb != null)
            {
                if (!String.IsNullOrEmpty(SessionId))
                {
                    OrderFromDb.SessionId = SessionId;
                }
                if (!String.IsNullOrEmpty(PaymentIntentId))
                {
                    OrderFromDb.PaymentInetetId = PaymentIntentId;
                }
            }
        }
    }
}
