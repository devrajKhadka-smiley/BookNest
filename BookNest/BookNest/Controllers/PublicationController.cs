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
        private readonly AppDbContext dbContext;

        public PublicationController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult GetAllPublications()
        {
            List<Publication> publicationsList = dbContext.Publications.ToList();
            return Ok(publicationsList);
        }

        [HttpGet("{id}")]
        public IActionResult GetPublicationById(Guid id)
        {
            var publication = dbContext.Publications.FirstOrDefault(p => p.PublicationId == id);

            if (publication == null)
            {
                return NotFound($"Publication with ID {id} not found.");
            }

            return Ok(publication);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdatePublication(Guid id, Publication updatedPublication)
        {
            var publication = dbContext.Publications.FirstOrDefault(p => p.PublicationId == id);

            if (publication == null)
            {
                return NotFound($"Publication with ID {id} not found.");
            }

            publication.Name = updatedPublication.Name;

            dbContext.SaveChanges();

            return Ok(publication);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeletePublication(Guid id)
        {
            int rowsAffected = dbContext.Publications.Where(p => p.PublicationId == id).ExecuteDelete();

            return rowsAffected > 0
                ? Ok("Deleted successfully")
                : NotFound("Id not found");
        }
    }
}