namespace AppHub.Application.DTOs.User;

public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsEmailVerified { get; set; }
}

