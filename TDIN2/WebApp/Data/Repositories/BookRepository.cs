using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;

namespace WebApp.Data
{
    public class BookRepository : Repository<Book>
    {
        public BookRepository(ApplicationContext context) : base(context)
        {

        }

        public Book GetWithRelated(int id)
        {
            return Context.Books
                .FirstOrDefault(b => b.Id == id);
        }

        public List<Book> GetListWithRelated()
        {
            return Context.Books
                .ToList();
        }

        public Book GetByName(string title)
        {
            return Context.Books
                .FirstOrDefault(b => b.Title == title);
        }
    }
}
