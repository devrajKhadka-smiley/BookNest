using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Announcement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnnouncementController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
                return BadRequest("End date must be after start date");

            var announcement = new Announcement
            {
                Title = dto.Title,
                Message = dto.Message,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Announcement Created",
                Data = announcement
            });
        }

        [HttpGet("active-announcement")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReadAnnouncementDto>>> GetActiveAnnouncements()
        {
            var currentTime = DateTime.UtcNow;

            var active = await _context.Announcements
                .Where(a => a.StartDate <= currentTime && a.EndDate >= currentTime)
                .OrderBy(a => a.StartDate)
                .Select(a => new ReadAnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
                })
                .ToListAsync();

            return Ok(active);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnnouncement(Guid id, [FromBody] UpdateAnnouncementDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
                return BadRequest("End date must be after start date.");

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return NotFound("Announcement not found");

            announcement.Title = dto.Title;
            announcement.Message = dto.Message;
            announcement.StartDate = dto.StartDate;
            announcement.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Announcement updated successfully",
                Data = announcement
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return NotFound("Announcement not found.");

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return Ok("Announcement deleted successfully.");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReadAnnouncementDto>>> GetAllAnnouncements()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.StartDate)
                .Select(a => new ReadAnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
                })
                .ToListAsync();

            return Ok(announcements);
        }

    }
}
