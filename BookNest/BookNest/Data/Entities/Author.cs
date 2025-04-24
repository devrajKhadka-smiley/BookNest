using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Author
    {
        [Key]
        public Guid AuthorId { get; set; }

        [Required]
        [MaxLength(150)]
        public string? Name { get; set; }
    }
}
