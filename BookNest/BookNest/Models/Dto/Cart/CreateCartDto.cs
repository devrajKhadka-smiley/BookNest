namespace BookNest.Controllers
{
    public class CreateCartDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}
