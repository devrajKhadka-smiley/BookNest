using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long UserId { get; set; }
        public string MembershipId { get; set; }
        public bool OrderReceived { get; set; } = false;
        public string? ClaimCode { get; set; }
        public User? User { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Order()
        {
            Status = "In Process";
        }
    }
}
