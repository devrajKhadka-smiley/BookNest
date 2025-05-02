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
        private readonly AppDbContext dbContext;

        public BadgeController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult GetAllBadges()
        {
            List<Badge> badgesList = dbContext.Badges.ToList();
            return Ok(badgesList);
        }

        [HttpGet("{id}")]
        public IActionResult GetBadgeById(Guid id)
        {
            var badge = dbContext.Badges.FirstOrDefault(b => b.Id == id);

            if (badge == null)
            {
                return NotFound($"Badge with ID {id} not found.");
            }

            return Ok(badge);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateBadge(Guid id, Badge updatedBadge)
        {
            var badge = dbContext.Badges.FirstOrDefault(b => b.Id == id);

            if (badge == null)
            {
                return NotFound($"Badge with ID {id} not found.");
            }

            badge.BadgeName = updatedBadge.BadgeName;

            badge.Books = updatedBadge.Books;

            dbContext.SaveChanges();

            return Ok(badge);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteBadge(Guid id)
        {
            int rowsAffected = dbContext.Badges.Where(b => b.Id == id).ExecuteDelete();

            if (rowsAffected == 0)
            {
                return NotFound($"Badge with ID {id} not found.");
            }

            return Ok($"Badge with ID {id} deleted successfully.");
        }
    }
}
