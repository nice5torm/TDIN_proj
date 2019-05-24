using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data.Repositories;
using Common.Models;

namespace WebApp.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Clients = new ClientRepository(context);
            Orders = new OrderRepository(context);
            Books = new BookRepository(context);
            Sales = new SaleRepository(context);
        }

        public ClientRepository Clients { get; private set; }

        public OrderRepository Orders { get; private set; }

        public BookRepository Books { get; private set; }

        public SaleRepository Sales { get; private set; }


        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
