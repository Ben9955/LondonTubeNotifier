using System.ComponentModel.DataAnnotations;

namespace LondonTubeNotifier.WebApi.Dtos
{ 
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username or Email is required")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}

