namespace AppHub.Application.DTOs.Application;

public class CreateApplicationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AppKey { get; set; }
    public bool IsActive { get; set; } = true;
}

