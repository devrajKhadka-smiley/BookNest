using BookNest.Data;
using BookNest.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BadgeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BadgeController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task <IActionResult> GetAllBadges()
        {
            List<Badge> badgesList = await _context.Badges.ToListAsync();
            return Ok(badgesList);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetBadgeById(Guid id)
        {
            var badge = await _context.Badges.FindAsync(id);

            if (badge == null)
            {
                return NotFound($"Badge with ID {id} not found.");
            }

            return Ok(badge);
        }

        [HttpPut("update/{id}")]
        public async Task <IActionResult> UpdateBadge(Guid id, Badge updatedBadge)
        {
            var badge = await _context.Badges.FindAsync(id);

            if (badge == null)
            {
                return NotFound($"Badge with ID {id} not found.");
            }

            badge.BadgeName = updatedBadge.BadgeName;

            badge.Books = updatedBadge.Books;

            _context.SaveChanges();

            return Ok(badge);
        }
    }
}
