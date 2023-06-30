using System.ComponentModel.DataAnnotations;

namespace A3_API_Project.Models.IDP.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
