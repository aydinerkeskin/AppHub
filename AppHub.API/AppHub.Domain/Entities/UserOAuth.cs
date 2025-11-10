namespace AppHub.Domain.Entities;

public class UserOAuth
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProviderId { get; set; }
    public string ProviderUserId { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual OAuthProvider Provider { get; set; } = null!;
}

