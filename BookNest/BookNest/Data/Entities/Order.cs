using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long UserId { get; set; }
        public bool OrderReceived { get; set; } = false;
        public List<OrderItem> Items { get; set;} = new List<OrderItem>();
        public User? User { get; set; }
    }
}
