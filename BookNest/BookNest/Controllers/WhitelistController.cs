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
        public async Task<ActionResult<IEnumerable<WhitelistDto>>> GetUserWhitelist(int userId)
        {
            var whitelist = await _context.Whitelists
                .Where(w => w.UserId == userId)
                .Select(w => new WhitelistDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    BookId = w.BookId,
                    //AddedDate = w.AddedDate
                })
                .ToListAsync();

            return Ok(whitelist);
        }

        // POST: api/Whitelist
        [HttpPost]
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

        // DELETE: api/Whitelist/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWhitelist(int id)
        {
            var entry = await _context.Whitelists.FindAsync(id);
            if (entry == null)
                return NotFound();

            _context.Whitelists.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
