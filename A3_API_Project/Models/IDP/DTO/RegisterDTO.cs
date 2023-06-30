using System.ComponentModel.DataAnnotations;

namespace A3_API_Project.Models.IDP.DTO
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não confere.")]
        public string ConfirmPassword { get; set; }
    }
}
