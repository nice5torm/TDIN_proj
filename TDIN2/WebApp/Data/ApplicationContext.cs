using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;


namespace WebApp.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Sale> Sales { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Order>()                    
            //    .Property(b => b.DispatchedDate)
            //    .HasDefaultValueSql("CONVERT(date, GETDATE())");


            //modelBuilder.Entity<Utilizador>()             //usar para o client?
            //    .HasIndex(u => u.Username)
            //    .IsUnique();
        }

        
    }

}
