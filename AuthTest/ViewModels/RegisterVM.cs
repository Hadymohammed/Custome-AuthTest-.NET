using Microsoft.Build.Framework;

namespace AuthTest.ViewModels
{
    public class RegisterVM
    {
        [Required]
       public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }

    }
}
