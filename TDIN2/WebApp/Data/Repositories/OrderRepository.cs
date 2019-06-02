using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(ApplicationContext context) : base(context)
        {

        }

        public Order GetWithRelated(int id)
        {
            return Context.Orders
                .Include(o => o.Client)
                .Include(o => o.Book)
                .FirstOrDefault(o => o.GUID == id);
        }

        public List<Order> GetListWithRelatedClient(int Cid)
        {
            return Context.Orders
                .Include(o => o.Client)
                .Include(o => o.Book)
                .Where(o => o.ClientId == Cid).ToList();
        }

        public List<Order> GetListWithRelatedBook(int Bid)
        {
            return Context.Orders
                .Include(o => o.Client)
                .Include(o => o.Book)
                .Where(o => o.BookId == Bid).ToList();
        }

        public List<Order> GetListWithRelated()
        {
            return Context.Orders
                .Include(o => o.Book)
                .Include(o => o.Client)
                .ToList();
        }
    }
}
