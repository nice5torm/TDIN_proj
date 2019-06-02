using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using Common.Models;
using Common.Services;

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
                List<Order> lo = unitOfWork.Orders.GetListWithRelatedClient(id);
                if (lo != null)
                {
                    return Ok(lo);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet("GetOrdersBook")]
        public ActionResult<List<Order>> GetOrdersBook(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                List<Order> lo = unitOfWork.Orders.GetListWithRelatedBook(id);
                if(lo != null)
                {
                    return Ok(lo);
                }
                else
                {
                    return NotFound();
                }
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
        public ActionResult<Order> CreateOrder([FromBody] Order model)     
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( "Pedido Inválido" );
            }
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    //EmailSender.SendEmail(model.Client.Email, "Order Creation Information",
                    //        "You just ordered the book: " + model.Book.Title + " the cost is " + model.Book.Price + ". You ordered " + model.Quantity + ". The total price is " + model.Quantity * model.Book.Price + " . The Order status is Dispatched at " + model.DispatchedDate.ToShortDateString());

                    unitOfWork.Orders.Add(model);
                    unitOfWork.Complete();

                    return Ok(model);
                }
                catch (Exception)
                {
                    return BadRequest( "Erro desconhecido" );
                }
            }
            
        }
        [HttpPost("CreateOrderWithMessage")]
        public ActionResult<Order> CreateOrderWithMessage([FromBody] Order model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Pedido Inválido");
            }
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    //MessageQueue.SendMessageToWarehouse(model.Book.Title,model.Quantity, model.Id);

                    //EmailSender.SendEmail(model.Client.Email, "Order Creation Information",
                    //        "You just ordered the book: " + model.Book.Title + " the cost is " + model.Book.Price + ". You ordered " + model.Quantity + ". The total price is " + model.Quantity * model.Book.Price + " . The Order status is  Waiting Expedition");

                    unitOfWork.Orders.Add(model);
                    unitOfWork.Complete();

                    return Ok(model);
                }
                catch (Exception)
                {
                    return BadRequest("Erro desconhecido");
                }
            }

        }
   

        [HttpPost("CreateOrderWithMessageAndStock")]
        public ActionResult<Order> CreateOrderWithMessageAndStock([FromBody] Order model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Pedido Inválido");
            }
            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    //MessageQueue.SendMessageToWarehouse(model.Book.Title, model.Quantity + 10, model.Id);

                    //EmailSender.SendEmail(model.Client.Email, "Order Creation Information",
                    //        "You just ordered the book: " + model.Book.Title + " the cost is " + model.Book.Price + ". You ordered " + model.Quantity + ". The total price is " + model.Quantity * model.Book.Price + " . The Order status is  Waiting Expedition");

                    unitOfWork.Orders.Add(model);
                    unitOfWork.Complete();

                    return Ok(model);
                }
                catch (Exception)
                {
                    return BadRequest("Erro desconhecido");
                }
            }

        }

        [HttpPut("EditOrder")]
        public ActionResult<string> EditOrder([FromBody] Order model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Pedido Inválido");
            }

            using (UnitOfWork unitOfWork = new UnitOfWork(_context))
            {
                try
                {
                    unitOfWork.Orders.Update(model);
                    unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest("Erro ao editar");
                }
            }
        }

        [HttpDelete("DeleteOrder")]
        public ActionResult<string> DeleteOrder(int id)
        {
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

