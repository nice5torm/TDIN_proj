using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class ClientController : Controller
    {
        private readonly ApplicationContext _context;

        public ClientController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("GetClients")]
        public ActionResult<List<Client>> GetClients()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Clients.GetListWithRelated());
            }
        }

        [HttpGet("GetClient")]
        public ActionResult<Client> GetClient(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                Client c = unitOfWork.Clients.GetWithRelated(id);
                if (c != null)
                {
                    return Ok(c);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost("CreateClient")]
        public ActionResult<string> CreateClient([FromBody] Client model)     
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    "Pedido Inválido" 
                );
            }
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    unitOfWork.Clients.Add(model);
                    unitOfWork.Complete();
                    return Ok();
                }

                catch (Exception)
                {
                    return BadRequest( "Erro desconhecido" );
                }
            }
        }

        [HttpPut("EditClient")]
        public ActionResult<string> EditClient([FromBody] Client model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Pedido Inválido" );
            }

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    unitOfWork.Clients.Update(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest("Erro ao editar" );
                }
            }
        }

        [HttpDelete("DeleteClient")]
        public ActionResult<string> DeleteClient(int id)
        {

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    Client c = unitOfWork.Clients.GetWithRelated(id);

                    unitOfWork.Clients.Remove(c);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest( "Erro ao apagar");
                }
            }
        }
    }
}
