using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Author
{
    public class CreateAuthorDto
    {
        [Required]
        public string? AuthorName { get; set; }
    }
}
