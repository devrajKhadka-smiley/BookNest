using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNest.Data.Entities
{
    public class Whitelist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        // Navigation property
        public User User { get; set; }

        // Navigation property
        public Book Book { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
