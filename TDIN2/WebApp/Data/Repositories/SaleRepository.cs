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

        //public Sale GetWithRelated(int id)
        //{
        //    return Context.Sales
        //        .FirstOrDefault(s => s.ClientId == id);
        //}

        //public Sale GetWithRelated(int id)
        //{
        //    return Context.Sales
        //        .FirstOrDefault(s => s.BookId == id);
        //}

        public List<Sale> GetListWithRelated()
        {
            return Context.Sales
                .ToList();
        }
    }
}
