using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Publication
    {
        [Key]
        public Guid PublicationId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string? PublicationName { get; set; }

    }
}
