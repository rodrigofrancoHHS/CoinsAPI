using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersContext _context;

        public OrdersController(OrdersContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersDTO>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();

            var responseItems = orders.Select(o => new OrdersDTO
            {
                id = o.id,
                user_id = o.user_id,
                email = o.email,
                phone = o.phone,
                address = o.address,
                payment_method = o.payment_method,
                total_price = o.total_price,
                created_at = o.created_at,
                products = o.products,
                type = o.type,
            }).ToList();

            return Ok(responseItems);
        }






        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Orders>>> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders.Where(p => p.user_id == userId).ToListAsync();

            if (orders.Count == 0)
            {
                return NotFound();
            }

            return Ok(orders);
        }






        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersDTO>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDTO = new OrdersDTO
            {
                id = order.id,
                user_id = order.user_id,
                email = order.email,
                phone = order.phone,
                address = order.address,
                payment_method = order.payment_method,
                total_price = order.total_price,
                created_at = order.created_at,
                type = order.type,
            };

            return Ok(orderDTO);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrdersDTO orderDTO)
        {
            var order = new Orders
            {
                user_id = orderDTO.user_id,
                email = orderDTO.email,
                phone = orderDTO.phone,
                address = orderDTO.address,
                payment_method = orderDTO.payment_method,
                total_price = orderDTO.total_price,
                created_at = DateTime.Now,
                products = orderDTO.products, // Define o valor do campo products
                type = 1
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            orderDTO.id = order.id;
            orderDTO.created_at = order.created_at;

            return Ok(); // Retornar explicitamente OkResult
        }
       


        [HttpPost("ChangeOrderType/{id}")]
        public async Task<IActionResult> ChangeOrderType(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(); // Encomenda não encontrada
            }

            // Alterar o valor do campo 'type'
            order.type = order.type == 0 ? 1 : 0;

            await _context.SaveChangesAsync();

            return Ok(); // Sucesso na alteração do tipo da encomenda
        }


        [HttpPost("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(id);
        }

    }
}