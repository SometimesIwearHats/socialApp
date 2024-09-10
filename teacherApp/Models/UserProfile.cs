using System.ComponentModel.DataAnnotations;

namespace socialApp.Models
{
    public class UserProfile
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();  
        public string Name { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;  
        public string Bio { get; set; } = string.Empty;  
        public string ProfilePictureUrl { get; set; } = string.Empty;  
    }

}
