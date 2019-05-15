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

        public DbSet<Order> Order { get; set; }

        public DbSet<Client> Client { get; set; }

        public DbSet<Book> Book { get; set; }

        public DbSet<Stock> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()                    //fazer para o DispachedDate?
                .Property(b => b.WaitingDate)
                .HasDefaultValueSql("CONVERT(date, GETDATE())");



            //modelBuilder.Entity<Utilizador>()             //usar para o client?
            //    .HasIndex(u => u.Username)
            //    .IsUnique();
        }

        
    }

}
