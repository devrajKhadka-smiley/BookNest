using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Publication;
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
        public async Task<IActionResult> GetAllPublications()
        {
            List<Publication> publicationList = await _context.Publications.ToListAsync();

            if (publicationList == null || publicationList.Count == 0)
                return NotFound("No publication found");

            return Ok(publicationList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublicationById(Guid id)
        {
            Publication? publication = await _context.Publications.FindAsync(id);

            if (publication == null)
                return NotFound($"Publication with ID {id} not found.");

            return Ok(publication);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublication(CreatePublicationDto dto)
        {
            Publication publication = new Publication
            {
                PublicationName = dto.PublicationName
            };

            _context.Publications.Add(publication);
            await _context.SaveChangesAsync();

            return Ok(publication);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublication(Guid id, UpdatePublicationDto dto)
        {
            Publication? existingPublication = await _context.Publications.FindAsync(id);

            if (existingPublication == null)
                return NotFound($"Publication with ID {id} not found.");

            existingPublication.PublicationName = dto.PublicationName;
            await _context.SaveChangesAsync();

            return Ok(existingPublication);
        }
    }
}