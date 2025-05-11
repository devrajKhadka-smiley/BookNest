using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Announcement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }
        [StringLength(1000)]
        public string? Message { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
