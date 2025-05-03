using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Badge
{
    public class BadgeDto
    {
        public Guid BadgeId { get; set; }
        [Required]
        public string? BadgeName { get; set; }
    }
}
