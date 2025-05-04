namespace BookNest.Models.Dto.Book
{
    public class UpdateBookDto : CreateBookDto
    {
        public Guid BookId { get; set; }
    }
}
