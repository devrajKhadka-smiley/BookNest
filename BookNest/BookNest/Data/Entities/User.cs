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
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public string? MemberShipId { get; set; }
    }
}
