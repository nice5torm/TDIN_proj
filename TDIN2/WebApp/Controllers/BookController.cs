using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{

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

        [HttpPost("CreateBook")]
        public ActionResult<Result> CreateBook([FromBody] Book model)     //what is this? from body?
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Result
                {
                    Errors = new List<string> { "Pedido Inválido" }
                });
            }

            //string role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

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
                    return BadRequest(new Result
                    {
                        Errors = new List<string> { "Erro desconhecido" }
                    });
                }
            }
               
        }

        [HttpPut("EditBook")]
        public ActionResult<Result> EditBook([FromBody] Book model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Result
                {
                    Errors = new List<string> { "Pedido Inválido" }
                });
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
                    return BadRequest(new Result
                    {
                        Errors = new List<string> { "Erro ao editar" }
                    });
                }
            }
        }

        [HttpDelete("DeleteBook")]
        public ActionResult<Result> DeleteBook(int id)
        {
            //string role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

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
                    return BadRequest(new Result
                    {
                        Errors = new List<string> { "Erro ao apagar" }
                    });
                }
            }
        }

    }
}
