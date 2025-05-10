using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long UserId { get; set; }
        public bool OrderReceived { get; set; } = false;
        // public List<OrderItem> Items { get; set;} = new List<OrderItem>();
        public string? ClaimCode { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public User? User { get; set; }

        // ✅ New property to store final order amount (after discount if any)
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public Order()
        {
            Status = "In Process";
        }
    }
}
