using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Book
{
    public class BookDetailDto
    {
        public Guid BookId { get; set; }
        [Required]
        public string? BookTitle { get; set; }
        [Required]
        public string? BookISBN { get; set; }
        [Required]
        public string? BookDescription { get; set; }
        public int BookStock { get; set; }
        public float BookRating { get; set; }
        [Required]
        public string? BookFormat { get; set; }
        public int BookReviewCount { get; set; }
        [Required]
        public List<string>? AuthorName { get; set; }
        public Guid BookPublicationId { get; set; }
        public string? BookLanguage { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string? PublicationName { get; set; }
        public List<string>? Genres { get; set; }
        public List<string>? Badges { get; set; }
        public List<Guid>? AuthorIds { get; set; }
        public List<Guid>? GenreIds { get; set; }
        public List<Guid>? BadgeIds { get; set; }
        public decimal BookPrice { get; set; }
        public decimal BookFinalPrice { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        public decimal BookDiscountedPrice { get; set; }

    }
}
