using System.ComponentModel.DataAnnotations;

namespace LondonTubeNotifier.WebApi.Dtos
{ 
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username or Email is required")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

