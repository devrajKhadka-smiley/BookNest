using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Genre
    {
        [Key]
        public Guid GenreId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? GenreName { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
