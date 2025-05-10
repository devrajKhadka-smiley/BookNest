namespace BookNest.Models.Dto.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public string MemberShipId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> OrderDetails { get; set; }
    }
}
