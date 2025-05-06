using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Publication
{
    public class UpdatePublicationDto
    {
        [Required]
        public string? PublicationName { get; set; }
    }
}
