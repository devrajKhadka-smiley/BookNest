using Microsoft.AspNetCore.Mvc;
using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhitelistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WhitelistController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Whitelist/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserWhitelist(long userId)
        {
            var whitelist = await _context.Whitelists
                .Where(w => w.UserId == userId)
                .Include(w => w.Book)
                .Select(w => new
                {
                    w.Id,
                    w.UserId,
                    w.BookId,
                    BookTitle = w.Book.BookTitle,
                    BookPrice = w.Book.BookPrice,
                    BookPublisher = w.Book.Publication.PublicationName,
                })
                .ToListAsync();

            return Ok(whitelist);
        }

        // POST: api/Whitelist/add/{userId}
        [HttpPost("add/{userId}")]
        public async Task<IActionResult> AddToWhitelist(long userId, [FromBody] WhitelistDto dto)
        {
            // Check if book exists
            var bookExists = await _context.Books.AnyAsync(b => b.BookId == dto.BookId);
            if (!bookExists)
            {
                return NotFound("Book not found.");
            }

            // Check if already in whitelist
            bool alreadyExists = await _context.Whitelists
                .AnyAsync(w => w.UserId == userId && w.BookId == dto.BookId);

            if (alreadyExists)
            {
                return Conflict(new { message = "Book already exists in wishlist." });
            }

            var whitelist = new Whitelist
            {
                UserId = userId,
                BookId = dto.BookId,
                AddedDate = DateTime.UtcNow
            };

            _context.Whitelists.Add(whitelist);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book added to wishlist." });
        }

        // DELETE: api/Whitelist/remove/{userId}/{bookId}
        [HttpDelete("remove/{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFromWhitelist(long userId, Guid bookId)
        {
            var entry = await _context.Whitelists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

            if (entry == null)
            {
                return NotFound("Entry not found in wishlist.");
            }

            _context.Whitelists.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book successfully removed from wishlist." });
        }
    }
}
