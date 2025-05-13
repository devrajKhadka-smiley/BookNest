using Microsoft.AspNetCore.Mvc;
using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<WhitelistDto>>> GetUserWhitelist(int userId)
        {
            var wishlist = await _context.Whitelists
                .Where(w => w.UserId == userId)
                .Include(w => w.Book) // Include related book
                .Select(w => new WhitelistDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    BookId = w.BookId,

                    // Book info
                    BookTitle = w.Book.BookTitle,
                    BookPrice = w.Book.BookPrice,
                    BookDiscountedPrice = w.Book.BookDiscountedPrice,
                    BookReviewCount = w.Book.BookReviewCount,
                    BookRating = w.Book.BookRating,
                    BookStock = w.Book.BookStock,
                    OnSale = w.Book.IsOnSale,
                    DiscountPercentage = w.Book.DiscountPercentage,
                    ImageBase64 = w.Book.ImageData != null ? Convert.ToBase64String(w.Book.ImageData) : null
                })
                .ToListAsync();

            return Ok(wishlist);
        }


        // POST: api/Whitelist
        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<WhitelistDto>> AddToWhitelist(WhitelistDto dto)
        {
            bool alreadyExists = await _context.Whitelists
        .AnyAsync(w => w.UserId == dto.UserId && w.BookId == dto.BookId);

            if (alreadyExists)
            {
                return Conflict(new { message = "Product already exists in wishlist." });
            }

            var whitelist = new Whitelist
            {
                UserId = dto.UserId,
                BookId = dto.BookId,
                AddedDate = DateTime.UtcNow
            };

            _context.Whitelists.Add(whitelist);
            await _context.SaveChangesAsync();

            dto.Id = whitelist.Id;
            //dto.AddedDate = whitelist.AddedDate;

            return CreatedAtAction(nameof(GetUserWhitelist), new { userId = dto.UserId }, dto);
        }

        [HttpDelete("{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFromWishlist(int userId, Guid bookId)
        {
            var entry = await _context.Whitelists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

            if (entry == null)
                return NotFound(new { message = "Item not found in wishlist." });

            _context.Whitelists.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
