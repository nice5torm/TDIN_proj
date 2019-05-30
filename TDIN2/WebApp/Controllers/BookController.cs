using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using Common.Models;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class BookController : Controller
    {
        private readonly ApplicationContext _context;

        public BookController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("GetBooks")]
        public ActionResult<List<Book>> GetBooks()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Books.GetListWithRelated());
            }
        }

        [HttpGet("GetBook")]
        public ActionResult<Book> GetBook(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                Book b = unitOfWork.Books.GetWithRelated(id);
                if (b != null)
                {
                    return Ok(b);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet("GetBookByTitle")]
        public ActionResult<Book> GetBookByTitle(string title)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                Book b = unitOfWork.Books.GetByName(title);
                if (b != null)
                {
                    return Ok(b);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost("CreateBook")]
        public ActionResult<string> CreateBook([FromBody] Book model)     
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( "Pedido Inválido" );
            }
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    unitOfWork.Books.Add(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest( "Erro desconhecido" );
                }
            }
               
        }

        [HttpPut("EditBook")]
        public ActionResult<string> EditBook([FromBody] Book model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( "Pedido Inválido" );
            }

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {

                    unitOfWork.Books.Update(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest( "Erro ao editar" );
                }
            }
        }

        [HttpDelete("DeleteBook")]
        public ActionResult<string> DeleteBook(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    Book b = unitOfWork.Books.GetWithRelated(id);

                    unitOfWork.Books.Remove(b);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest( "Erro ao apagar" );
                }
            }
        }

    }
}
