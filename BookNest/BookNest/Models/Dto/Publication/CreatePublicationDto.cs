using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Publication
{
    public class CreatePublicationDto
    {
        [Required]
        public string? PublicationName { get; set; }
    }
}
