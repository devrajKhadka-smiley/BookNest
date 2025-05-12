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
        [Required]
        public string? AuthorName { get; set; }
        [Required]
        public string? PublicationName { get; set; }
        public List<string>? Genres { get; set; }
        public List<string>? Badges { get; set; }
        public decimal BookPrice { get; set; }
        public decimal BookFinalPrice { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        public decimal BookDiscountedPrice { get; set; }

    }
}
