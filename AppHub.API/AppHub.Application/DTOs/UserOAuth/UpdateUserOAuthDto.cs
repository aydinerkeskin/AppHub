namespace AppHub.Application.DTOs.UserOAuth;

public class UpdateUserOAuthDto
{
    public int? UserId { get; set; }
    public int? ProviderId { get; set; }
    public string? ProviderUserId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
}

