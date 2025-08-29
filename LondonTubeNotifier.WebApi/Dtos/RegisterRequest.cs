using System.ComponentModel.DataAnnotations;

namespace LondonTubeNotifier.WebApi.Dtos
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
