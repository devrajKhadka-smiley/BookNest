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
        public decimal? BookPrice { get; set; }
        [Required]
        public decimal? BookFinalPrice { get; set; }
        public decimal? BookDiscountedPrice { get; set; }
        [Required]
        public int? BookReviewCount { get; set; }
        [Required]
        public float? BookRating { get; set; }
        [Required]
        public List<string>? AuthorName { get; set; }
        [Required]
        public string? PublicationName { get; set; }
        public List<string>? Genres { get; set; }


        //-------------by Dev--
        public float? BookStock { get; set; }
        public float? SoldPiece { get; set; }
        public bool? OnSale { get; set; }
    }
}
