using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace A3_API_Project.Models.IDP.DTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Lembrar de mim?")]
        public bool RememberMe { get; set; }
    }
}
