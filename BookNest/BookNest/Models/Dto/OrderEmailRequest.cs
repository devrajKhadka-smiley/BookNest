using BookNest.Data.Entities;

namespace BookNest.Models.Dto
{
    public class OrderEmailRequest
    {
        public Data.Entities.Order Order { get; set; }
        public User User { get; set; }
    }

}
