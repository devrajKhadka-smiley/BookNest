using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Badge
{
    public class CreateBadgeDto
    {
        [Required]
        public string? BadgeName { get; set; }
    }
}
