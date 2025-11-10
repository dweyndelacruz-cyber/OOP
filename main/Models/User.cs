using System.ComponentModel.DataAnnotations;

namespace main.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string ContactNumber { get; set; } = string.Empty;
        
        [Required]
        public string HouseStreet { get; set; } = string.Empty;
        
        [Required]
        public string Barangay { get; set; } = string.Empty;
        
        public string City { get; set; } = "Navotas City";
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "Customer";
        
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsEmailVerified { get; set; } = false;
    }
}