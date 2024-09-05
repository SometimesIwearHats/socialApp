using System.ComponentModel.DataAnnotations;

namespace socialApp.Models
{
    public class UserProfile
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();  // Initialize with a default value
        public string Name { get; set; } = string.Empty;  // Initialize with a default value
        public string Email { get; set; } = string.Empty;  // Initialize with a default value
        public string Bio { get; set; } = string.Empty;  // Initialize with a default value
        public string ProfilePictureUrl { get; set; } = string.Empty;  // Initialize with a default value
    }

}
