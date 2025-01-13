using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IOrderHeaderRepositry : IRepositry<OrderHeader>
    {
        public void Update(OrderHeader entity);
        public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus=null);
        public void UpdateStripePaymentId(int id, string SessionId, string PaymentIntentId);
    }
}
