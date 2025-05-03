using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    [Index(nameof(BadgeName), IsUnique = true)]
    public class Badge
    {
        [Key]
        public Guid BadgeId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string BadgeName { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
