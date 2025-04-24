using System.ComponentModel.DataAnnotations;

namespace BookNest.Models.Dto
{
    public class RegisterUserDto
    {
        [Required]
        [DeniedValues("admin", "superuser", "staff", "root", ErrorMessage = "Reserse username of BookNest")]
        public string? UserName { get; set; }

        [Required]
        public string? Firstname { get; set; }

        [Required]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Password Didn't Match")]
        public string? ConfirmPassword { get; set; }
    }
}
