﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry.IRepositry
{
    public interface IUnitOfWork
    {
        ICategoryRepositry category { get; }
        public void Save();
    }
}
