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
                List<Sale> s = unitOfWork.Sales.GetListWithRelatedBook(id);
                if(s != null)
                {
                    return Ok(s);
                }
                else
                {
                    return NotFound();
                }
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
        public ActionResult<string> CreateSale([FromBody] Sale model)     
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( "Pedido Inválido" );
            }


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
                    return BadRequest( "Erro desconhecido");
                }
               
            }
               
        }


        [HttpDelete("DeleteSale")]
        public ActionResult<string> DeleteSale(int id)
        {
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
                    return BadRequest( "Erro ao apagar"  );
                }
            }
        }
    }
}
