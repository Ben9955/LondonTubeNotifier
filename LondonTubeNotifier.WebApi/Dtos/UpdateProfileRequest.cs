namespace LondonTubeNotifier.WebApi.Dtos
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PushNotifications { get; set; }
        public bool EmailNotifications { get; set; }

    }
}
