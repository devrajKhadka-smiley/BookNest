using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    [Index(nameof(BadgeName), IsUnique = true)]
    public class Badge
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public required string BadgeName { get; set; }
        public ICollection<Book> Books { get; set;}
    }
}
