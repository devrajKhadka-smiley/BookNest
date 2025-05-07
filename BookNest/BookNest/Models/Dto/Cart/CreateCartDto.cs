namespace BookNest.Models.Dto.Cart
{
    public class CreateCartDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}
