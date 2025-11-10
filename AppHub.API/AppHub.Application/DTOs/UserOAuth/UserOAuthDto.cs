namespace AppHub.Application.DTOs.UserOAuth;

public class UserOAuthDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProviderId { get; set; }
    public string ProviderUserId { get; set; } = string.Empty;
    public DateTime? TokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

