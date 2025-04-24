using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Publication
    {
        [Key]
        public Guid PublicationId { get; set; }

        [Required]
        [MaxLength(150)]
        public string? Name { get; set; }

    }
}
