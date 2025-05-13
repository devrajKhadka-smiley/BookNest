using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            var hasPurchased = await _context.Orders
                .Where(o => o.UserId == dto.UserId && o.OrderReceived)
                .SelectMany(o => o.OrderItems)
                .AnyAsync(oi => oi.BookId == dto.BookId);

            if (!hasPurchased)
                return BadRequest("You can only review books you've received");
            
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.UserId == dto.UserId && r.BookId == dto.BookId);

            if (alreadyReviewed)
                return BadRequest("You've already reviewed this book");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                BookId = dto.BookId,
                Rating = dto.Rating,
                Comment = dto.Comment,
            };

            _context.Reviews.Add(review);

            var book = await _context.Books.FindAsync(dto.BookId);

            if (book != null)
            {
                var ratings = await _context.Reviews
                    .Where(r => r.BookId == dto.BookId)
                    .Select(r => r.Rating)
                    .ToListAsync();

                ratings.Add(dto.Rating);
                book.BookReviewCount = ratings.Count;
                book.BookRating = (float)ratings.Average();
            }

            await _context.SaveChangesAsync();

            return Ok("Review submitted successfully");
        }

        [HttpGet("book/{bookId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetReviewsForBook(Guid bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    UserName = r.User != null ? r.User.UserName : "Anonymous",
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}
