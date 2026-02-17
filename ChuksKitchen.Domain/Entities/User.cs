using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsVerified { get; set; } = false;
    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public RegistrationMethod RegistrationMethod { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime? OtpGeneratedAt { get; set; }
    public int FailedOtpAttempts { get; set; } = 0;

    // Navigation properties
    public int? ReferrerId { get; set; }
    public User? Referrer { get; set; }
    public ICollection<User> Referrals { get; set; } = new List<User>();
    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
