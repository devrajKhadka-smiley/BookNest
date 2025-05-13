using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Book;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAllBooks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 8,
            [FromQuery] string? searchTerm = null,
            [FromQuery] List<Guid>? authorIds = null,
            [FromQuery] List<Guid>? publicationIds = null,
            [FromQuery] List<Guid>? genreIds = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isAscending = true,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] List<string>? languages = null,
            [FromQuery] List<string>? formats = null,
            [FromQuery] Guid? badgeId = null,
            [FromQuery] float? minRating = null
        )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page Number and page size must be greater than 0");

            var query = _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.Author)
                .Include(b => b.Publication)
                .Include(b => b.Genres)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerSearch = searchTerm.Trim().ToLower();
                query = query.Where(b =>
                    b.BookTitle!.ToLower().Contains(lowerSearch) ||
                    b.BookDescription!.ToLower().Contains(lowerSearch) ||
                    b.BookISBN!.ToLower().Contains(lowerSearch)
                );
            }

            if (authorIds != null && authorIds.Any())
                query = query.Where(b => b.Author!.Any(a => authorIds.Contains(a.AuthorId)));

            if (publicationIds != null && publicationIds.Any())
                query = query.Where(b => publicationIds.Contains(b.BookPublicationId));

            if (genreIds != null && genreIds.Any())
                query = query.Where(b => b.Genres!.Any(g => genreIds.Contains(g.GenreId)));

            if (minPrice.HasValue)
                query = query.Where(b => b.BookFinalPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(b => b.BookFinalPrice <= maxPrice.Value);

            if (languages != null && languages.Any())
            {
                List<string> loweredLanguages = languages.Select(l => l.Trim().ToLower()).ToList();
                query = query.Where(b => loweredLanguages.Contains(b.BookLanguage!.ToLower()));
            }

            if (formats != null && formats.Any())
            {
                var loweredFormats = formats.Select(f => f.Trim().ToLower()).ToList();
                query = query.Where(b => loweredFormats.Contains(b.BookFormat!.ToLower()));
            }

            if (badgeId.HasValue)
                query = query.Where(b => b.Badges.Any(bd => bd.BadgeId == badgeId.Value));

            if (minRating.HasValue)
                query = query.Where(b => b.BookRating >= minRating.Value);

            int totalRecords = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "title" => isAscending
                    ? query.OrderBy(b => b.BookTitle)
                    : query.OrderByDescending(b => b.BookTitle),

                "date" or "publicationdate" => isAscending
                    ? query.OrderBy(b => b.BookAddedDate)
                    : query.OrderByDescending(b => b.BookAddedDate),

                "price" => isAscending
                    ? query.OrderBy(b => b.BookPrice)
                    : query.OrderByDescending(b => b.BookPrice),

                "popularity" => isAscending
                    ? query.OrderBy(b => b.BookSold)
                    : query.OrderByDescending(b => b.BookSold),

                _ => query.OrderBy(b => b.BookTitle)
            };


            var pagedBooks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = pagedBooks.Select(b => new ReadBookDto
            {
                BookId = b.BookId,
                BookTitle = b.BookTitle,
                BookISBN = b.BookISBN,
                BookPrice = b.BookPrice,
                BookFinalPrice = b.BookFinalPrice,
                BookDiscountedPrice = b.BookDiscountedPrice,
                BookReviewCount = b.BookReviewCount,
                BookRating = b.BookRating,
                AuthorName = b.Author!.Select(a => a.AuthorName!).ToList(),
                PublicationName = b.Publication != null ? b.Publication.PublicationName! : "Unknown",
                Genres = b.Genres!.Select(g => g.GenreName!).ToList(),
                IsDeleted = b.IsDeleted,
                OnSale = b.IsOnSale,
                BookStock = b.BookStock,
                DiscountPercentage = b.DiscountPercentage,
            }).ToList();

            var response = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                AuthorIds = authorIds,
                PublicationIds = publicationIds,
                GenreIds = genreIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Languages = languages,
                Formats = formats,
                BadgeId = badgeId,
                MinRating = minRating,
                SortBy = sortBy,
                IsAscending = isAscending,
                Data = result
            };

            return Ok(response);
        }

        [HttpGet("adminGetAllBooks")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var allBooks = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publication)
                .Include(b => b.Genres)
                .Select(b => new ReadBookDto
                {
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    BookISBN = b.BookISBN,
                    BookPrice = b.BookPrice,
                    BookFinalPrice = b.BookFinalPrice,
                    BookDiscountedPrice = b.BookDiscountedPrice,
                    BookReviewCount = b.BookReviewCount,
                    BookRating = b.BookRating,
                    BookStock = b.BookStock,
                    SoldPiece = b.BookSold,
                    OnSale = b.IsOnSale,
                    DiscountPercentage = b.DiscountPercentage,
                    AuthorName = b.Author!.Select(a => a.AuthorName!).ToList(),
                    PublicationName = b.Publication != null ? b.Publication.PublicationName : "Unknown",
                    Genres = b.Genres != null ? b.Genres.Select(g => g.GenreName).ToList() : new List<string>(),
                    IsDeleted = b.IsDeleted,
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Books retrieved successfully",
                data = allBooks
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _context.Books
                .Where(b => !b.IsDeleted)
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
                BookStock = book.BookStock,
                BookRating = book.BookRating,
                BookFormat = book.BookFormat,
                BookReviewCount = book.BookReviewCount,
                BookPublicationId = book.BookPublicationId,
                BookLanguage = book.BookLanguage,
                PublicationName = book.Publication?.PublicationName ?? "Unknown",
                AuthorIds = book.Author!.Select(a => a.AuthorId).ToList(),
                GenreIds = book.Genres!.Select(g => g.GenreId).ToList(),
                BadgeIds = book.Badges!.Select(b => b.BadgeId).ToList(),
                BookPrice = book.BookPrice,
                BookFinalPrice = book.BookFinalPrice,
                IsOnSale = book.IsOnSale,
                DiscountStartDate = book.DiscountStartDate,
                DiscountEndDate = book.DiscountEndDate,
                BookDiscountedPrice = book.BookDiscountedPrice,
                IsDeleted = book.IsDeleted,
                DiscountPercentage = book.DiscountPercentage
            };

            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateBook(CreateBookDto dto)
        //{
        //    bool authorExists = await _context.Authors
        //        .AnyAsync(a => a.AuthorId == dto.BookAuthorId);

        //    if (!authorExists)
        //        return BadRequest("The specified author does not exist.");

        //    bool publicationExists = await _context.Publications
        //        .AnyAsync(p => p.PublicationId == dto.BookPublicationId);

        //    if (!publicationExists)
        //        return BadRequest("The specified publication does not exist.");

        //    var book = new Book
        //    {
        //        BookId = Guid.NewGuid(),
        //        BookTitle = dto.BookTitle,
        //        BookISBN = dto.BookISBN,
        //        BookDescription = dto.BookDescription,
        //        BookAuthorId = dto.BookAuthorId,
        //        BookPublicationId = dto.BookPublicationId,
        //        BookStock = dto.BookStock,
        //        BookPrice = dto.BookPrice,
        //        BookRating = dto.BookRating,
        //        BookLanguage = dto.BookLanguage,
        //        BookFormat = dto.BookFormat,
        //        BookSold = dto.BookSold,
        //        DiscountPercentage = dto.DiscountPercentage,
        //        BookReviewCount = dto.BookReviewCount,
        //        BookFinalPrice = dto.BookFinalPrice,
        //        IsOnSale = dto.IsOnSale,
        //        DiscountStartDate = dto.DiscountStartDate,
        //        DiscountEndDate = dto.DiscountEndDate,
        //        BookAddedDate = DateTime.UtcNow
        //    };

        //    if (dto.GenreIds != null && dto.GenreIds.Any())
        //    {
        //        book.Genres = await _context.Genres
        //            .Where(g => dto.GenreIds.Contains(g.GenreId))
        //            .ToListAsync();
        //    }

        //    if (dto.BadgeIds != null && dto.BadgeIds.Any())
        //    {
        //        book.Badges = await _context.Badges
        //            .Where(b => dto.BadgeIds.Contains(b.BadgeId))
        //            .ToListAsync();
        //    }

        //    _context.Books.Add(book);
        //    await _context.SaveChangesAsync();

        //    Book? createdBook = await _context.Books
        //        .Include(b => b.Author)
        //        .Include(b => b.Publication)
        //        .Include(b => b.Genres)
        //        .Include(b => b.Badges)
        //        .FirstOrDefaultAsync(b => b.BookId == book.BookId);

        //    BookDetailDto result = new BookDetailDto
        //    {
        //        BookId = book.BookId,
        //        BookTitle = book.BookTitle,
        //        BookISBN = book.BookISBN,
        //        BookDescription = book.BookDescription,
        //        AuthorName = book.Author != null ? string.Join(", ", book.Author.Select(a => a.AuthorName)) : "Unknown",
        //        PublicationName = book.Publication?.PublicationName ?? "Unknown",
        //        Genres = book.Genres!.Select(g => g.GenreName!).ToList(),
        //        Badges = book.Badges!.Select(b => b.BadgeName!).ToList(),
        //        BookPrice = book.BookPrice,
        //        BookFinalPrice = book.BookFinalPrice,
        //        IsOnSale = book.IsOnSale,
        //        DiscountStartDate = book.DiscountStartDate,
        //        DiscountEndDate = book.DiscountEndDate
        //    };

        //    return CreatedAtAction(nameof(GetBookById), new { id = result.BookId }, result);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookDto dto)
        {
            if (dto.AuthorIds == null || !dto.AuthorIds.Any())
                return BadRequest("At least one author must be specified.");

            // Check if all provided authors exist
            bool allAuthorsExist = await _context.Authors
                .Where(a => dto.AuthorIds.Contains(a.AuthorId))
                .CountAsync() == dto.AuthorIds.Count;

            if (!allAuthorsExist)
                return BadRequest("One or more specified authors do not exist.");

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
                BookPublicationId = dto.BookPublicationId,
                BookStock = dto.BookStock,
                BookPrice = dto.BookPrice,
                //BookRating = dto.BookRating,
                BookLanguage = dto.BookLanguage,
                BookFormat = dto.BookFormat,
                //BookSold = dto.BookSold,
                DiscountPercentage = dto.DiscountPercentage,
                //BookReviewCount = dto.BookReviewCount,
                BookFinalPrice = dto.BookFinalPrice,
                IsOnSale = dto.IsOnSale,
                DiscountStartDate = dto.DiscountStartDate,
                DiscountEndDate = dto.DiscountEndDate,
                BookAddedDate = DateTime.UtcNow

            };


            book.Author = await _context.Authors
                .Where(a => dto.AuthorIds.Contains(a.AuthorId))
                .ToListAsync();


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


            var createdBook = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publication)
                .Include(b => b.Genres)
                .Include(b => b.Badges)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);

            var result = new BookDetailDto
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle,
                BookISBN = book.BookISBN,
                BookDescription = book.BookDescription,
                AuthorName = createdBook!.Author != null
                    ? createdBook.Author.Select(a => a.AuthorName!).ToList()
                    : new List<string>(),
                PublicationName = createdBook.Publication?.PublicationName ?? "Unknown",
                Genres = createdBook.Genres?.Select(g => g.GenreName!).ToList() ?? new List<string>(),
                Badges = createdBook.Badges?.Select(b => b.BadgeName!).ToList() ?? new List<string>(),

                AuthorIds = createdBook.Author?.Select(a => a.AuthorId).ToList() ?? new List<Guid>(),
                GenreIds = createdBook.Genres?.Select(g => g.GenreId).ToList() ?? new List<Guid>(),
                BadgeIds = createdBook.Badges?.Select(b => b.BadgeId).ToList() ?? new List<Guid>(),
                BookPublicationId = book.BookPublicationId,
                BookLanguage = book.BookLanguage,
                BookPrice = book.BookPrice,
                BookFinalPrice = book.BookFinalPrice,
                IsOnSale = book.IsOnSale,
                DiscountStartDate = book.DiscountStartDate,
                DiscountEndDate = book.DiscountEndDate,
                BookDiscountedPrice = book.BookDiscountedPrice,
                IsDeleted = book.IsDeleted,
                DiscountPercentage = book.DiscountPercentage
            };


            return CreatedAtAction(nameof(GetBookById), new { id = result.BookId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto dto)
        {

            if (id != dto.BookId)
                return BadRequest("Book ID mismatch.");

            Book? book = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Badges)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return NotFound("Book not found.");

            bool allAuthorsExist = await _context.Authors
                .Where(a => dto.AuthorIds.Contains(a.AuthorId))
                .CountAsync() == dto.AuthorIds.Count;
            bool publicationExists = await _context.Publications.AnyAsync(p => p.PublicationId == dto.BookPublicationId);

            if (!allAuthorsExist || !publicationExists)
                return BadRequest("Invalid Author or Publication.");

            book.BookTitle = dto.BookTitle;
            book.BookISBN = dto.BookISBN;
            book.BookDescription = dto.BookDescription;
            //book.BookAuthorId = dto.BookAuthorId;
            book.BookPublicationId = dto.BookPublicationId;
            book.BookStock = dto.BookStock;
            book.BookPrice = dto.BookPrice;
            //book.BookRating = dto.BookRating;
            book.BookLanguage = dto.BookLanguage;
            book.BookFormat = dto.BookFormat;
            //book.BookSold = dto.BookSold;
            book.DiscountPercentage = dto.DiscountPercentage;
            //book.BookReviewCount = dto.BookReviewCount;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            Book? book = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Badges)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return NotFound("Book not found.");

            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("available-formats")]
        public async Task<IActionResult> GetAvailableFormats()
        {
            var formats = await _context.Books
                .Where(b => !b.IsDeleted)
                .Where(b => !string.IsNullOrEmpty(b.BookFormat))
                .Select(b => b.BookFormat!.Trim())
                .Distinct()
                .ToListAsync();

            return Ok(formats);
        }

        [HttpGet("available-languages")]
        public async Task<IActionResult> GetAvailableLanguages()
        {
            var languages = await _context.Books
                .Where(b => !b.IsDeleted)
                .Where(b => !string.IsNullOrEmpty(b.BookLanguage))
                .Select(b => b.BookLanguage!.Trim())
                .Distinct()
                .ToListAsync();

            return Ok(languages);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreBook(Guid id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return NotFound("Book not found.");

            book.IsDeleted = false;
            await _context.SaveChangesAsync();

            return Ok("Book restored.");
        }

    }
}
