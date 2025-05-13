using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Genre;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAllGenres()
        {
            List<Genre> genreList = await _context.Genres.ToListAsync();

            if (genreList == null || genreList.Count == 0)
                return NotFound("No Genre Found");

            return Ok(genreList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenre(Guid id)
        {
            Genre? genre = await _context.Genres.FindAsync(id);

            if (genre == null)
                return NotFound("Genre Id Not Found");

            return Ok(genre);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGenre(CreateGenreDto dto)
        {
            Genre genre = new Genre
            {
                GenreName = dto.GenreName,
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return Ok(genre);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGenre(Guid id, UpdateGenreDto dto)
        {
            Genre? existingGenre = await _context.Genres.FindAsync(id);

            if (existingGenre == null)
                return NotFound("Genre Not Found");

            existingGenre.GenreName = dto.GenreName;
            await _context.SaveChangesAsync();

            return Ok(existingGenre);
        }
    }
}
