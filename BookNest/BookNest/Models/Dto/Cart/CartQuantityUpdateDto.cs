namespace BookNest.Models.Dto.Cart
{
    public class CartQuantityUpdateDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}
