using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Author
{
    public class AuthorDto
    {
        public Guid AuthorId { get; set; }
        [Required]
        public string? AuthorName { get; set; }
    }
}
