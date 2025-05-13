using BookNest.Data;
using BookNest.Models.Dto.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("stats-cards")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminStatsCards()
        {
            var now = DateTime.UtcNow;

            var staffRoleId = await _context.Roles
                .Where(r => r.Name == "Staff")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var stats = new AdminDashboardDto
            {
                TotalBooks = await _context.Books.CountAsync(),

                ActiveAnnouncements = await _context.Announcements.CountAsync(a =>
                    a.StartDate <= now && a.EndDate >= now),

                ActiveDiscounts = await _context.Books.CountAsync(b =>
                    b.IsOnSale &&
                    b.DiscountStartDate <= now &&
                    b.DiscountEndDate >= now),

                TotalStaff = await _context.UserRoles
                    .Where(ur => ur.RoleId == staffRoleId)
                    .CountAsync()
            };

            return Ok(stats);
        }

        [HttpGet("highest-selling-books")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHighestSellingBooks()
        {
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.BookSold)
                .Take(5)
                .Select(b => new HighestSellingDto
                {
                    Title = b.BookTitle,
                    Rating = b.BookRating,
                    Language = b.BookLanguage,
                    Publication = b.Publication != null ? b.Publication.PublicationName : "N/A"
                })
                .ToListAsync();

            return Ok(books);
        }


    }
}
