namespace AppHub.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Application? Application { get; set; }
    public virtual ICollection<UserOAuth> UserOAuths { get; set; } = new List<UserOAuth>();
}

