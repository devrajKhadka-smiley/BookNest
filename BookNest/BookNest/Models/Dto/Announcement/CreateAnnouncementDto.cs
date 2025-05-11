using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Announcement
{
    public class CreateAnnouncementDto
    {
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }
        [Required]
        [StringLength(1000)]
        public string? Message { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
