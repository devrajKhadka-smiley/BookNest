namespace BookNest.Data.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid BookId { get; set; }
        public Book? Book { get; set; }


        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }

        public Guid OrderId { get; set; }
        public Order? Order { get; set; }



    }
}
