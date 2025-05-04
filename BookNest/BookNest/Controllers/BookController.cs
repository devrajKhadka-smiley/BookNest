using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Book;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto dto)
        {
            if (id != dto.BookId)
                return BadRequest("Book ID mismatch.");

            var book = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Badges)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return NotFound("Book not found.");

            var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == dto.BookAuthorId);
            var publicationExists = await _context.Publications.AnyAsync(p => p.PublicationId == dto.BookPublicationId);

            if (!authorExists || !publicationExists)
                return BadRequest("Invalid Author or Publication.");

            book.BookTitle = dto.BookTitle;
            book.BookISBN = dto.BookISBN;
            book.BookDescription = dto.BookDescription;
            book.BookAuthorId = dto.BookAuthorId;
            book.BookPublicationId = dto.BookPublicationId;
            book.BookStock = dto.BookStock;
            book.BookPrice = dto.BookPrice;
            book.BookRating = dto.BookRating;
            book.BookLanguage = dto.BookLanguage;
            book.BookFormat = dto.BookFormat;
            book.BookSold = dto.BookSold;
            book.DiscountPercentage = dto.DiscountPercentage;
            book.BookReviewCount = dto.BookReviewCount;
            book.BookFinalPrice = dto.BookFinalPrice;
            book.IsOnSale = dto.IsOnSale;
            book.DiscountStartDate = dto.DiscountStartDate;
            book.DiscountEndDate = dto.DiscountEndDate;


            book.Genres = dto.GenreIds != null && dto.GenreIds.Any()
                ? await _context.Genres.Where(g => dto.GenreIds.Contains(g.GenreId)).ToListAsync()
                : new List<Genre>();

            book.Badges = dto.BadgeIds != null && dto.BadgeIds.Any()
                ? await _context.Badges.Where(b => dto.BadgeIds.Contains(b.BadgeId)).ToListAsync()
                : new List<Badge>();

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
