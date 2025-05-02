using BookNest.Data;
using BookNest.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PublicationController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task <IActionResult> GetAllPublications()
        {
            List<Publication> publicationsList = await _context.Publications.ToListAsync();
            return Ok(publicationsList);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetPublicationById(Guid id)
        {
            var publication = await _context.Publications.FindAsync(id);

            if (publication == null)
            {
                return NotFound($"Publication with ID {id} not found.");
            }

            return Ok(publication);
        }

        [HttpPut("update/{id}")]
        public async Task <IActionResult> UpdatePublication(Guid id, Publication updatedPublication)
        {
            var publication = await _context.Publications.FindAsync(id);

            if (publication == null)
            {
                return NotFound($"Publication with ID {id} not found.");
            }

            publication.Name = updatedPublication.Name;

            _context.SaveChanges();

            return Ok(publication);
        }
    }
}