using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    //[Index(nameof(Email), IsUnique = true)]
    //[Index(nameof(UserName), IsUnique = true)]
    //[Index(nameof(PhoneNumber), IsUnique = true)]
    //[Index(nameof(MemberShipId), IsUnique = true)]
    public class User : IdentityUser<long>
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Address { get; set; }
        public required string MemberShipId { get; set; }
    }
}
