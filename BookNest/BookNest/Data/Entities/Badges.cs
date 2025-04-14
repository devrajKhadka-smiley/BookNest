using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    [Index(nameof(BadgeName), IsUnique = true)]
    public class Badges
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public required string BadgeName { get; set; }
    }
}
