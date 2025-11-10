namespace AppHub.Application.DTOs.Application;

public class UpdateApplicationDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AppKey { get; set; }
    public bool? IsActive { get; set; }
}

