using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_ManyToManyRel.Data;
using WebApi_ManyToManyRel.Models;

namespace WebApi_ManyToManyRel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDBContext _db;
        public BooksController(AppDBContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            var books = await _db.Books
                .Include(b => b.BookOrders)    // load join-rows (book -> bookorders)
                    .ThenInclude(bo => bo.Order)
                .AsNoTracking()
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            var book = await _db.Books
                .Include(b => b.BookOrders)
                    .ThenInclude(bo => bo.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = book.BookId }, book);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book updated)
        {
            if (id != updated.BookId) return BadRequest("Id mismatch");
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Title = updated.Title;
            book.Price = updated.Price;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
