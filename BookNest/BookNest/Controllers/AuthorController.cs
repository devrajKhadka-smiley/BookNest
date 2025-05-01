using BookNest.Data;
using BookNest.Data.Entities;
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

        public AuthorController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            List<Author> authorList = await _context.Authors.ToListAsync();

            if (authorList == null || authorList.Count == 0)
            {
                return NotFound("No author found");
            }
            return Ok(authorList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthors(Guid id)
        {
            Author? author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound("Author ID Not Found");
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(Author), new { id = author.AuthorId }, author);
            return Ok(author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, Author updatedAuthor)
        {
            if (id != updatedAuthor.AuthorId)
            {
                return BadRequest("ID mismatch");
            }

            Author? existingAuthor = await _context.Authors.FindAsync(id);

            if (existingAuthor == null)
            {
                return NotFound("Author Not Found");
            }

            existingAuthor.AuthorName= updatedAuthor.AuthorName;
            await _context.SaveChangesAsync();

            return Ok(existingAuthor);
        }
    }
}
