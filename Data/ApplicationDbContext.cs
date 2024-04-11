using Microsoft.EntityFrameworkCore;
using Supershop.Models;

namespace Supershop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<items> items { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<items>().HasData(
                new items
                {
                    Id = 1,
                    Name = "Chokbar",
                    Count = 15,
                    location = "F1",
                    Type = "Icecream",
                    Price = 35.50,
                    ImageUrl = "/images/pepsi.png" // Store relative URL
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
