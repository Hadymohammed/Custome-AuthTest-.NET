using System.ComponentModel.DataAnnotations;

namespace AuthTest.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "User name")]
        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
