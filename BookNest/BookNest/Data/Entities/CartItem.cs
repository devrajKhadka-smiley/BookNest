namespace BookNest.Data.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public Guid CartId { get; set; }

        //// Navigation
        public Book? Book { get; set; }
        public Cart? Cart { get; set; }
    }
}
 