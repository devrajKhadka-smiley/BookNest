using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Genre
{
    public class UpdateGenreDto
    {
        [Required]
        public string? GenreName { get; set; }
    }
}
