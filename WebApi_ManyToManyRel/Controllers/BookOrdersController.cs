using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_ManyToManyRel.Data;
using WebApi_ManyToManyRel.Models;

namespace WebApi_ManyToManyRel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookOrdersController : ControllerBase
    {
        private readonly AppDBContext _db;
        public BookOrdersController(AppDBContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] BookOrder bookOrder)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Ensure Order and Book exist
            var orderExists = await _db.Orders.AnyAsync(o => o.OrderId == bookOrder.OrderId);
            if (!orderExists) return NotFound($"Order {bookOrder.OrderId} not found");

            var bookExists = await _db.Books.AnyAsync(b => b.BookId == bookOrder.BookId);
            if (!bookExists) return NotFound($"Book {bookOrder.BookId} not found");

            // Composite key in AppDBContext is (OrderId, BookId)
            var existing = await _db.BookOrders.FindAsync(bookOrder.OrderId, bookOrder.BookId);
            if (existing != null)
            {
                existing.Quantity = bookOrder.Quantity;
            }
            else
            {
                _db.BookOrders.Add(bookOrder);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Remove book from order by query params
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] int orderId, [FromQuery] int bookId)
        {
            var bo = await _db.BookOrders.FindAsync(orderId, bookId);
            if (bo == null) return NotFound();

            _db.BookOrders.Remove(bo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Get all BookOrders (join table)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookOrder>>> GetAll()
        {
            var list = await _db.BookOrders
                .Include(bo => bo.Book)
                .Include(bo => bo.Order)
                .AsNoTracking()
                .ToListAsync();

            return Ok(list);
        }
    }
}
