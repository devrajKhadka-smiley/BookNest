using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Author;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            List<Author> authorList = await _context.Authors.ToListAsync();

            if (authorList == null || authorList.Count == 0)
                return NotFound("No author found");

            return Ok(authorList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            Author? author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound("Author ID Not Found");

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto dto)
        {
            Author author = new Author
            {
                AuthorName = dto.AuthorName,
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return Ok(author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, UpdateAuthorDto dto)
        {
            Author? existingAuthor = await _context.Authors.FindAsync(id);

            if (existingAuthor == null)
                return NotFound("Author Not Found");

            existingAuthor.AuthorName = dto.AuthorName;
            await _context.SaveChangesAsync();

            return Ok(existingAuthor);
        }
    }
}
