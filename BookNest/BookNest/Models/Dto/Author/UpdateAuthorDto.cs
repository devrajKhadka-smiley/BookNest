using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Author
{
    public class UpdateAuthorDto
    {
        [Required]
        public string? AuthorName { get; set; }
    }
}
