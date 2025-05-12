using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    public class Book
    {
        [Key]
        public Guid BookId { get; set; }

        [MaxLength(200)]
        public string? BookTitle { get; set; }

        [MaxLength(20)]
        public string? BookISBN { get; set; }

        [MaxLength(2000)]
        public string? BookDescription { get; set; }

        //public Guid BookAuthorId { get; set; }

        //public Author? Author { get; set; }

        public int BookStock { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal BookPrice { get; set; }

        public float BookRating { get; set; }

        [MaxLength(50)]
        public string? BookLanguage { get; set; }

        [MaxLength(50)]
        public string? BookFormat { get; set; }

        public Guid BookPublicationId { get; set; }

        public Publication? Publication { get; set; }

        public int BookSold { get; set; }

        public float DiscountPercentage { get; set; }

        public DateTime BookAddedDate { get; set; }

        public int BookReviewCount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal BookFinalPrice { get; set; }

        public bool IsOnSale { get; set; }

        public DateTime? DiscountStartDate { get; set; }

        public DateTime? DiscountEndDate { get; set; }

        public ICollection<Genre>? Genres { get; set; }

        public ICollection<Badge>? Badges { get; set; }

        public ICollection<Author>? Author { get; set; }

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public decimal BookDiscountedPrice
        {
            get
            {
                var now = DateTime.UtcNow;

                if (IsOnSale
                    && DiscountStartDate.HasValue
                    && DiscountEndDate.HasValue
                    && now >= DiscountStartDate.Value
                    && now <= DiscountEndDate.Value)
                {
                    return BookPrice * (1 - (decimal)DiscountPercentage / 100);
                }

                return BookPrice;
            }
        }
    }
}
