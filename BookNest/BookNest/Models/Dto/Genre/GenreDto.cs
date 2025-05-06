using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Genre
{
    public class GenreDto
    {
        public Guid GenreId { get; set; }
        [Required]
        public string? GenreName { get; set;}
    }
}
