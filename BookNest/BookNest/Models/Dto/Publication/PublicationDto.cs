using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto.Publication
{
    public class PublicationDto
    {
        public Guid PublicationId { get; set; }
        [Required]
        public string? PublicationName { get; set; }
    }
}
