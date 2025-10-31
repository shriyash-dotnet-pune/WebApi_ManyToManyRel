using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_ManyToManyRel.Data;
using WebApi_ManyToManyRel.Models;

namespace WebApi_ManyToManyRel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDBContext _db;
        public OrdersController(AppDBContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _db.Orders
                .Include(o => o.BookOrders)
                    .ThenInclude(bo => bo.Book)
                .AsNoTracking()
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _db.Orders
                .Include(o => o.BookOrders)
                    .ThenInclude(bo => bo.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        // Create order with optionally included BookOrders in body
        [HttpPost]
        public async Task<ActionResult<Order>> Create([FromBody] Order order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // If BookOrders provided, ensure referenced books exist
            if (order.BookOrders != null && order.BookOrders.Any())
            {
                foreach (var bo in order.BookOrders)
                {
                    var exists = await _db.Books.AnyAsync(b => b.BookId == bo.BookId);
                    if (!exists) return BadRequest($"Book {bo.BookId} not found");
                }
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = order.OrderId }, order);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Order updated)
        {
            if (id != updated.OrderId) return BadRequest("Id mismatch");
            var order = await _db.Orders
                .Include(o => o.BookOrders)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            order.CustomerName = updated.CustomerName;
            order.OrderDate = updated.OrderDate;

            // Replace BookOrders if provided (simple replace strategy)
            if (updated.BookOrders != null)
            {
                // validate books exist
                foreach (var bo in updated.BookOrders)
                {
                    if (!await _db.Books.AnyAsync(b => b.BookId == bo.BookId))
                        return BadRequest($"Book {bo.BookId} not found");
                }

                _db.BookOrders.RemoveRange(order.BookOrders);
                order.BookOrders = updated.BookOrders;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
