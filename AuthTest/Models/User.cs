using AuthTest.Data;
using System.ComponentModel.DataAnnotations;

namespace AuthTest.Models
{
    public class User: IAbstractUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }

    }
}
