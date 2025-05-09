namespace BookNest.Models.Dto.Review
{
    public class CreateReviewDto
    {
        public long UserId { get; set; }
        public Guid BookId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
