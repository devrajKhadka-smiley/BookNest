using BookNest.Data;
using BookNest.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            List<Genre> genreList = await _context.Genres.ToListAsync();

            if (genreList == null  || genreList.Count == 0)
            {
                return NotFound("No Genre Found");
            }

            return Ok(genreList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenre(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound("Genre Id Not Found");
            }

            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(GetGenre), new { id = genre.GenreId }, genre);
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(Guid id, Genre updatedGenre)
        {
            if (id != updatedGenre.GenreId)
            {
                return BadRequest("ID mismatch");
            }

            Genre? existingGenre = await _context.Genres.FindAsync(id);

            if (existingGenre == null)
            {
                return NotFound("Genre Not Found");
            }

            existingGenre.GenreName = updatedGenre.GenreName;
            await _context.SaveChangesAsync();

            return Ok(existingGenre);
        }
    }
}
