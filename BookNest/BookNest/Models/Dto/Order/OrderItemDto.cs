namespace BookNest.Models.Dto.Order
{
    public class OrderItemDto
    {
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid OrderId { get; set; }
        public Data.Entities.Order? Order { get; set; }
        public decimal SubTotal
        {
            get; set;
        }
    }
}
