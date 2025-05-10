using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    public class User : IdentityUser<long>
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public string? MemberShipId { get; set; }

        // Track number of successful orders for stackable discount
        public int SuccessfulOrderCount { get; set; } = 0;

        //-- Navigation Properties
        public Cart Cart { get; set; }
        public Order Order { get; set; }

        // ✅ Check if user is a member (MembershipId present)
        [NotMapped]
        public bool IsMember => !string.IsNullOrEmpty(MemberShipId);

        // ✅ Optional: Get basic 5% member discount
        public decimal GetMemberDiscount()
        {
            const decimal MemberDiscountRate = 0.05m;
            return IsMember ? MemberDiscountRate : 0m;
        }
    }
}
