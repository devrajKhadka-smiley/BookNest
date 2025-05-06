using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Badge
{
    public class UpdateBadgeDto
    {
        [Required]
        public string? BadgeName { get; set; }
    }
}
