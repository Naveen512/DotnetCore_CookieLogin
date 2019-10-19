using System.ComponentModel.DataAnnotations;

namespace CookieAuth.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsPersistant { get; set; }
    }
}
