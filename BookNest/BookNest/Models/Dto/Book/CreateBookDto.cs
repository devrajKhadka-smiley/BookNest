using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Book
{
    public class CreateBookDto
    {
        [Required]
        public string BookTitle { get; set; } = string.Empty;
        [Required]
        public string? BookISBN { get; set; }
        public string? BookDescription { get; set; }
        //[Required]
        //public Guid BookAuthorId { get; set; }
        [Required]
        public Guid BookPublicationId { get; set; }
        public int BookStock { get; set; }
        public decimal BookPrice { get; set; }
        //public float BookRating { get; set; }
        public string? BookLanguage { get; set; }
        public string? BookFormat { get; set; }
        //public int BookSold { get; set; }
        public float DiscountPercentage { get; set; }
        //public int BookReviewCount { get; set; }
        public decimal BookFinalPrice { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public List<Guid>? GenreIds { get; set; }
        public List<Guid>? BadgeIds { get; set; }
        public List<Guid> AuthorIds { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
