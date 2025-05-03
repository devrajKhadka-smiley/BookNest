using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Genre
{
    public class CreateGenreDto
    {
        [Required]
        public string? GenreName { get; set; }
    }
}
