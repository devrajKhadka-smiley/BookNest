using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    [Index(nameof(MemberShipId), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public required string UserName { get; set; }

        [StringLength(100)]
        public required string FullName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [Phone]
        public required string PhoneNumber { get; set; }

        [StringLength(20)]
        public required string UserType { get; set; }

        public required string Password { get; set; }

        public required string MemberShipId { get; set; }
    }
}
