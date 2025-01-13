using MauiBook.DataAccess.Data;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.Repositry
{
    public class ProductRepositry : Repositry<Product>, IProductRepositry
    {
        private readonly ApplicationDbContext _Db;
        public ProductRepositry(ApplicationDbContext Db) : base(Db) 
        {
            _Db = Db;
        }

        public void Update(Product entity)
        {
            var obj = _Db.products.FirstOrDefault(p => p.Id == entity.Id);

            if(obj != null)
            {
                obj.Title = entity.Title;
                obj.Description = entity.Description;
                obj.Author = entity.Author;
                obj.ISBN = entity.ISBN;
                obj.ListPrice = entity.ListPrice;
                obj.Price = entity.Price;
                obj.Price50 = entity.Price50;
                obj.Price100 = entity.Price100;
                obj.category = entity.category;
                obj.CategoryId = entity.CategoryId;

                if(obj.ImageUrl != null)
                {
                    obj.ImageUrl = entity.ImageUrl;
                }
            }
        }
    }
}
