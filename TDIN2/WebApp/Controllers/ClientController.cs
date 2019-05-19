﻿using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<Result> CreateClient([FromBody] Client model)     //what is this? from body?
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
                unitOfWork.Clients.Add(model);
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

        [HttpPut("EditClient")]
        public ActionResult<Result> EditClient([FromBody] Client model)
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
                    unitOfWork.Clients.Update(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest(new Result
                    {
                        Errors = new List<string> { "Erro ao editar, outro utilizador deve ter editado entretanto" }
                    });
                }
            }
        }

        [HttpDelete("DeleteClient")]
        public ActionResult<Result> DeleteClient(int id)
        {
            //string role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

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
                    return BadRequest(new Result
                    {
                        Errors = new List<string> { "Erro ao apagar" }
                    });
                }
            }
        }
    }
}
