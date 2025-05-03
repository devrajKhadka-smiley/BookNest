using BookNest.Data;
using BookNest.Models.Dto.Book;
using Microsoft.AspNetCore.Http;
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
    }
}
