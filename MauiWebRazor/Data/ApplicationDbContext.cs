using MauiWebRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace MauiWebRazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "C1", DisplayOrder = 1 },
                new Category { Id = 2, Name = "C2", DisplayOrder = 2 },
                new Category { Id = 3, Name = "C3", DisplayOrder = 3 }
                );
        }

        public DbSet<Category> categories {  get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

    }
}
