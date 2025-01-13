﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader Order { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
