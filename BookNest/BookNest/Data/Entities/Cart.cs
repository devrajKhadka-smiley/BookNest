using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    [Table("UserCart")]
    public class Cart
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //--Foreign Key
        public long UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        //--Navigatin Property
        public User? User { get; set; }

    }
}
