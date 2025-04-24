using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto
{
    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }
    }
}
