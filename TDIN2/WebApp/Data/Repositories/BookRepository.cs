using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Data
{
    public class BookRepository : Repository<Book>
    {
        public BookRepository(ApplicationContext context) : base(context)
        {

        }

        public List<Book> GetListWithRelated()
        {
            return Context.Books
                .ToList();
        }

        public Book GetByName(string title)
        {
            return Context.Books
                .FirstOrDefault(l => l.Title == title);
        }
    }
}
