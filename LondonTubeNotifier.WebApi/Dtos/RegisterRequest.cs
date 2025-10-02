using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LondonTubeNotifier.WebApi.Dtos
{
    public class RegisterRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool PushNotifications { get; set; }
        public bool EmailNotifications { get; set; }
    }
}

