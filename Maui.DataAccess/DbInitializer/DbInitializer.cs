using MauiBook.DataAccess.Data;
using MauiBook.Models;
using MauiBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _Db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public DbInitializer(ApplicationDbContext Db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _Db = Db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        void IDbInitializer.Initialize()
        {
            // migration if they are not updated

            try
            {
                if(_Db.Database.GetPendingMigrations().Count() > 0)
                {
                    _Db.Database.Migrate();
                }
            }catch (Exception ex)
            {

            }
            // create roles
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                // if they are not created then will create admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "Hayy Al Bakarly",
                    State = "Babil",
                    PostalCode = "5101",
                    City = "Al Hillah",
                }, "Admin123*").GetAwaiter().GetResult();
                ApplicationUser user = _Db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;

        }
    }
}
