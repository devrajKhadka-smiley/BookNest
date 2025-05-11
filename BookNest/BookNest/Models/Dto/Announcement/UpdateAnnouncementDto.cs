using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Announcement
{
    public class UpdateAnnouncementDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
