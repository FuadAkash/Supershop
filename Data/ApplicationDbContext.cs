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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<items>().HasData(
                new items { Id=1, Name="Pepsi", Count=0, location="A1", Type="Softdrinks"},
                new items { Id = 2, Name = "Fanta", Count = 0, location = "A1", Type = "Softdrinks" },
                new items { Id = 3, Name = "7up", Count = 0, location = "A1", Type = "Softdrinks" },
                new items { Id = 4, Name = "Cockacola", Count = 0, location = "A1", Type = "Softdrinks" }

                );
        }
    }

}
