using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Data.Repositories
{
    public class SaleRepository : Repository<Sale>
    {
        public SaleRepository(ApplicationContext context) : base(context)
        {

        }

        public Sale GetWithRelated(int id)
        {
            return Context.Sales
                .FirstOrDefault(s => s.GUID == id);
        }

        public List<Sale> GetListWithRelatedBook(int Bid)
        {
            return Context.Sales
                .Where(s => s.BookId == Bid).ToList();
        }

        public List<Sale> GetListWithRelated()
        {
            return Context.Sales
                .ToList();
        }
    }
}
