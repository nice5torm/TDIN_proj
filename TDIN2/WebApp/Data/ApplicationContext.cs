﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;


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

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Livro", Price = 12, Amount = 2},
                new Book { Id = 2, Title = "Dicionário", Price = 20, Amount = 4}
                );
            modelBuilder.Entity<Client>().HasData(
                new Client { ID = 1, Name = "Maria", Address = "Rua 123", Email = "maria@mail.com", OrdersClient = new List<Order>() }
                );

        }
    }



}
