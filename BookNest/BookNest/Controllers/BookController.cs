using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Book;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publication)
                .Include(b => b.Genres)
                .Select(b => new ReadBookDto
                {
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    BookISBN = b.BookISBN,
                    AuthorName = b.Author!.AuthorName,
                    PublicationName = b.Publication!.PublicationName,
                    Genres = b.Genres!.Select(g => g.GenreName!).ToList()
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publication)
                .Include(b => b.Genres)
                .Include(b => b.Badges)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return NotFound("Book Not Found");

            BookDetailDto result = new BookDetailDto
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle,
                BookISBN = book.BookISBN,
                BookDescription = book.BookDescription,
                AuthorName = book.Author?.AuthorName ?? "Unknown",
                PublicationName = book.Publication?.PublicationName ?? "Unknown",
                Genres = book.Genres!.Select(g => g.GenreName!).ToList(),
                Badges = book.Badges!.Select(b => b.BadgeName!).ToList(),
                BookPrice = book.BookPrice,
                BookFinalPrice = book.BookFinalPrice,
                IsOnSale = book.IsOnSale,
                DiscountStartDate = book.DiscountStartDate,
                DiscountEndDate = book.DiscountEndDate
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookDto dto)
        {
            bool authorExists = await _context.Authors
                .AnyAsync(a => a.AuthorId == dto.BookAuthorId);

            if (!authorExists)
                return BadRequest("The specified author does not exist.");

            bool publicationExists = await _context.Publications
                .AnyAsync(p => p.PublicationId == dto.BookPublicationId);

            if (!publicationExists)
                return BadRequest("The specified publication does not exist.");

            var book = new Book
            {
                BookId = Guid.NewGuid(),
                BookTitle = dto.BookTitle,
                BookISBN = dto.BookISBN,
                BookDescription = dto.BookDescription,
                BookAuthorId = dto.BookAuthorId,
                BookPublicationId = dto.BookPublicationId,
                BookStock = dto.BookStock,
                BookPrice = dto.BookPrice,
                BookRating = dto.BookRating,
                BookLanguage = dto.BookLanguage,
                BookFormat = dto.BookFormat,
                BookSold = dto.BookSold,
                DiscountPercentage = dto.DiscountPercentage,
                BookReviewCount = dto.BookReviewCount,
                BookFinalPrice = dto.BookFinalPrice,
                IsOnSale = dto.IsOnSale,
                DiscountStartDate = dto.DiscountStartDate,
                DiscountEndDate = dto.DiscountEndDate,
                BookAddedDate = DateTime.UtcNow
            };

            if (dto.GenreIds != null && dto.GenreIds.Any())
            {
                book.Genres = await _context.Genres
                    .Where(g => dto.GenreIds.Contains(g.GenreId))
                    .ToListAsync();
            }

            if (dto.BadgeIds != null && dto.BadgeIds.Any())
            {
                book.Badges = await _context.Badges
                    .Where(b => dto.BadgeIds.Contains(b.BadgeId))
                    .ToListAsync();
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            Book? createdBook = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publication)
                .Include(b => b.Genres)
                .Include(b => b.Badges)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);

            BookDetailDto result = new BookDetailDto
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle,
                BookISBN = book.BookISBN,
                BookDescription = book.BookDescription,
                AuthorName = book.Author?.AuthorName ?? "Unknown",
                PublicationName = book.Publication?.PublicationName ?? "Unknown",
                Genres = book.Genres!.Select(g => g.GenreName!).ToList(),
                Badges = book.Badges!.Select(b => b.BadgeName!).ToList(),
                BookPrice = book.BookPrice,
                BookFinalPrice = book.BookFinalPrice,
                IsOnSale = book.IsOnSale,
                DiscountStartDate = book.DiscountStartDate,
                DiscountEndDate = book.DiscountEndDate
            };

            return CreatedAtAction(nameof(GetBookById), new {id = result.BookId}, result);
        }
    }
}
