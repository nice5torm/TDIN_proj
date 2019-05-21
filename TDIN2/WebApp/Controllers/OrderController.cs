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

    public class OrderController : Controller
    {
        private readonly ApplicationContext _context;

        public OrderController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("GetOrders")]
        public ActionResult<List<Order>> GetOrders()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Orders.GetListWithRelated());
            }
        }

        [HttpGet("GetOrdersClient")]
        public ActionResult<List<Order>> GetOrdersClient(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Orders.GetListWithRelatedClient(id));
            }
        }

        [HttpGet("GetOrdersBook")]
        public ActionResult<List<Order>> GetOrdersBook(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                return Ok(unitOfWork.Orders.GetListWithRelatedBook(id));
            }
        }

        [HttpGet("GetOrder")]
        public ActionResult<Order> GetOrder(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                Order o = unitOfWork.Orders.GetWithRelated(id);
                if (o != null)
                {
                    return Ok(o);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost("CreateOrder")]
        public ActionResult<string> CreateOrder([FromBody] Order model)     //what is this? from body?
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( "Pedido Inválido" );
            }

            //string role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    unitOfWork.Orders.Add(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest( "Erro desconhecido" );
                }
            }
            
        }

        [HttpDelete("DeleteOrder")]
        public ActionResult<string> DeleteOrder(int id)
        {
            //string role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    Order o = unitOfWork.Orders.GetWithRelated(id);

                    unitOfWork.Orders.Remove(o);
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
