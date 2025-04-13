using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? UserName { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string? Role { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
