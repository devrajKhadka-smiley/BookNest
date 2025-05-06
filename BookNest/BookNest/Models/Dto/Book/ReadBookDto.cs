using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Book
{
    public class ReadBookDto
    {
        public Guid BookId { get; set; }
        [Required]
        public string? BookTitle { get; set; }
        [Required]
        public string? BookISBN { get; set; }
        [Required]
        public string? AuthorName { get; set; }
        [Required]
        public string? PublicationName { get; set; }
        public List<string>? Genres { get; set; }
    }
}
