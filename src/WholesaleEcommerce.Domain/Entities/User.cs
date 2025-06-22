using System.ComponentModel.DataAnnotations;

namespace WholesaleEcommerce.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "wholesale"; // wholesale, retail, admin
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "active"; // active, inactive, suspended
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Новые поля для отслеживания онлайн статуса
    public bool IsOnline { get; set; }
    
    public DateTime? LastSeenAt { get; set; }
    
    public string? ConnectionId { get; set; }

    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
    
    public bool IsAdmin => Role == "admin";
    
    public bool IsActive => Status == "active";
    
    public bool IsWholesale => Role == "wholesale";
    
    public bool IsRetail => Role == "retail";

    // Navigation properties
    public List<Order> Orders { get; set; } = new();
} 