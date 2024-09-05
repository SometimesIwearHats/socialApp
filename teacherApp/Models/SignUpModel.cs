using System.ComponentModel.DataAnnotations;

namespace socialApp.Models
{
    public class SignUpModel
    {
        public SignUpModel() { }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
