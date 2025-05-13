namespace BookNest.Models.Dto
{
    public class WhitelistDto
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public Guid BookId { get; set; }

        //public DateTime AddedDate { get; set; }
        public string? BookTitle { get; set; }
        public decimal BookPrice { get; set; }
        public decimal? BookDiscountedPrice { get; set; }
        public int BookReviewCount { get; set; }
        public float BookRating { get; set; }
        public int BookStock { get; set; }
        public bool OnSale { get; set; }
        public float DiscountPercentage { get; set; }
    }
}
