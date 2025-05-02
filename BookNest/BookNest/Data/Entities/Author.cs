using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Author
    {
        [Key]
        public Guid AuthorId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string? AuthorName { get; set; }
    }
}
