using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Badge;
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
        public async Task<IActionResult> GetAllBadges()
        {
            List<Badge> badgeList = await _context.Badges.ToListAsync();

            if (badgeList == null || badgeList.Count == 0)
                return NotFound("No Badge Found");

            return Ok(badgeList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBadgeById(Guid id)
        {
            Badge? badge = await _context.Badges.FindAsync(id);

            if (badge == null)
                return NotFound($"Badge with ID {id} not found.");

            return Ok(badge);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBadge(CreateBadgeDto dto)
        {
            Badge badge = new Badge
            {
                BadgeName = dto.BadgeName
            };

            _context.Badges.Add(badge);
            await _context.SaveChangesAsync();

            return Ok(badge);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBadge(Guid id, UpdateBadgeDto dto)
        {
            Badge? existingBadge = await _context.Badges.FindAsync(id);

            if (existingBadge == null)
                return NotFound($"Badge with ID {id} not found.");

            existingBadge.BadgeName = dto.BadgeName;
            _context.SaveChanges();

            return Ok(existingBadge);
        }
    }
}
