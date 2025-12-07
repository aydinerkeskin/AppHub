namespace AppHub.Application.DTOs.User;

public class LoginRequestDto
{
    public int ApplicationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
