using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class SaleController : Controller
    {
        private readonly ApplicationContext _context;

        public SaleController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("GetSales")]
        public ActionResult<List<Sale>> GetSales()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Sales.GetListWithRelated());
            }
        }

        [HttpGet("GetSalesBook")]
        public ActionResult<List<Sale>> GetSalesBook(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Sales.GetListWithRelatedBook(id));
            }
        }

        [HttpGet("GetSale")]
        public ActionResult<Sale> GetSale(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                Sale s= unitOfWork.Sales.GetWithRelated(id);
                if (s != null)
                {
                    return Ok(s);
                }
                else
                {
                    return NotFound();
                }
            }
        }
       
        [HttpPost("CreateSale")]
        public ActionResult<Result> CreateSale([FromBody] Sale model)     //what is this? from body?
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
                    unitOfWork.Sales.Add(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch(Exception)
                {
                    return BadRequest(new Result
                    {
                        Errors = new List<string> { "Erro desconhecido" }
                    });
                }
               
            }
               
        }


        [HttpDelete("DeleteSale")]
        public ActionResult<Result> DeleteSale(int id)
        {
            //string role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    Sale s = unitOfWork.Sales.GetWithRelated(id);

                    unitOfWork.Sales.Remove(s);
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
