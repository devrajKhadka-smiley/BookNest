namespace BookNest.Data.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid BookId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
        public Book? Book { get; set; }
    }
}
